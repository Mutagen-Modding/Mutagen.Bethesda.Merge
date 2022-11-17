using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Archives;
using Microsoft.Extensions.FileSystemGlobbing;
using Noggog;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Skyrim = Mutagen.Bethesda.Skyrim;
using Fallout4 = Mutagen.Bethesda.Fallout4;
using Oblivion = Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins.Masters;

namespace MutagenMerger.Lib
{
    public static class AssetMerge<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
            where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
            where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
            where TMajorRecordGetter : class, IMajorRecordGetter
    {
        static MergeState<TMod, TModGetter> _mergeState = null!;
        static string _outputDir = "";
        static string _mergeName = "";
        static string temp = GetTemporaryDirectory();
        public static List<string> Rules
        {
            get
            {
                var rules = new List<string>() {"**/*.@(esp|esm|bsa|ba2|bsl)", "meta.ini",
                    "interface/translations/*.txt", "TES5Edit Backups/**/*",
                    "fomod/**/*", "screenshot?(s)/**/*", "scripts/source/*.psc", "source/scripts/*.psc"};

                _mergeState.ModsToMerge.ForEach(x =>
                {
                    rules.Add($"**/{x.Name.ToLower()}.seq");
                    rules.Add($"**/{x.Name.ToLower()}.ini");
                    rules.Add($"**/{x.Name.ToLower()}_DISTR.ini");
                    rules.Add($"**/{x.Name.ToLower()}_ANIO.ini");
                    rules.Add($"**/{x.Name.ToLower()}_SWAP.ini");
                    rules.Add($"**/{x.Name.ToLower()}_KID.ini");
                    rules.Add($"**/{x.FileName.String.ToLower()}/**/*");
                });
                return rules;
            }
        }

        public static void Handle(
            MergeState<TMod, TModGetter> mergeState)
        {
            _mergeState = mergeState;
            _outputDir = Path.GetDirectoryName(mergeState.OutputPath) ?? "";
            _mergeName = Path.GetFileName(mergeState.OutputPath);
            var matcher = new Matcher();
            matcher.AddIncludePatterns(new string[] { "**/*" });
            matcher.AddExcludePatterns(Rules);

            foreach (var mod in _mergeState.ModsToMerge)
            {

                var bsaPattern = mod.FileName.NameWithoutExtension + "*." + (_mergeState.Release == GameRelease.Fallout4 ? "b2a" : "bsa");
                string[] bsaFiles = Directory.GetFiles(_mergeState.DataPath, bsaPattern);

                // bsaFiles.ForEach(Console.WriteLine);

                foreach (string bsa in bsaFiles)
                {
                    ExtractBSA(bsa);
                }
                Console.WriteLine();

            }

            var matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(temp)));

            matches.Files.ForEach(x =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(_outputDir, x.Path)) ?? "");
                Console.WriteLine("            Copying extracted asset \"" + x.Path + "\"");
                File.Copy(Path.Combine(temp, x.Path), Path.Combine(_outputDir, x.Path));
            });

            foreach (var mod in _mergeState.ModsToMerge)
            {
                CopyAssets(_mergeState.DataPath, mod);
                CopyAssets(temp, mod);
            }

            BuildSeqFile(_mergeState.DataPath, temp, _mergeState.OutgoingMod);




            Directory.Delete(temp, true);
        }

        private static void BuildSeqFile(string dataPath, string temp, TMod outputMod)
        {
            var formIds = GetSeqQuests(outputMod);
            if (formIds.Count == 0) return;
            var fileName = _mergeName.Substring(0, _mergeName.Length - 4) + ".seq";
            var filePath = Path.Combine(_outputDir, "seq", fileName);
            var buffer = new byte[formIds.Count * sizeof(UInt32)];

            for (int i = 0; i < formIds.Count; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(formIds[i]), 0, buffer, i * 4, 4);
            }
            if (!BitConverter.IsLittleEndian) Array.Reverse(buffer);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "");
            File.WriteAllBytes(filePath, buffer);
            Console.WriteLine();
            Console.WriteLine("          Created SEQ file: " + fileName);

            // if (!formIds.length) return;
            // let filename = fh.getFileBase(merge.filename) + '.seq',
            //     filePath = `${ merge.dataPath}\\seq\\${ filename}`,
            // buffer = new Buffer(formIds.length * 4);
            // formIds.forEach((fid, n) => buffer.writeUInt32LE(fid, n * 4));
            // fh.jetpack.write(filePath, buffer);
            // progressLogger.log('Created SEQ file: ' + filePath);
        }

        private static List<UInt32> GetSeqQuests(TMod merge)
        {
            var masterColl = MasterReferenceCollection.FromPath(Path.Combine(_outputDir, _mergeName), _mergeState.Release);

            IGroup quests = _mergeState.Release switch
            {
                GameRelease.Oblivion => merge.GetTopLevelGroup<Oblivion.Quest>(),
                GameRelease.Fallout4 => merge.GetTopLevelGroup<Fallout4.Quest>(),
                _ => merge.GetTopLevelGroup<Skyrim.Quest>(),
            };

            List<UInt32> formIds = new List<UInt32>();

            if (quests.Count == 0) return formIds;

            foreach (var quest in quests.Records)
            {
                var fid = masterColl.GetFormID(quest.FormKey).Raw;
                formIds.Add(fid);
            }

            return formIds;
        }

        private static void CopyAssets(string path, ModKey mod)
        {
            CopyActorAssets(path, "textures/actors/character/facegendata/facetint", mod);
            CopyActorAssets(path, "meshes/actors/character/facegendata/facegeom", mod);
            CopyActorAssets(path, "sound/voice", mod);

            CopyTranslations(path, mod);
        }

        private static void ExtractBSA(string bsa)
        {
            Console.WriteLine();

            var reader = Archive.CreateReader(_mergeState.Release, bsa);
            var files = reader.Files.ToArray();
            for (int i = 0; i < files.Count(); i++)
            {
                var file = files[i];
                var filePath = file.Path.Replace("\\", "/").ToLower();
                Directory.CreateDirectory(Path.Combine(temp, Path.GetDirectoryName(filePath) ?? ""));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("          Extracting Archive \"" + Path.GetFileName(bsa) + "\" " + ((decimal)i / files.Count()).ToString("0.00%"));
                File.WriteAllBytes(Path.Combine(temp, filePath), file.GetBytes());

            }
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("          Extracting Archive \"" + Path.GetFileName(bsa) + "\" 100.00%");
        }

        private static void CopyTranslations(string dir, ModKey mod)
        {
            var path = "interface/translations/";
            var srcPath = Path.Combine(dir, path);
            var dstPath = Path.Combine(_outputDir, path);

            if (!Directory.Exists(srcPath)) return;

            foreach (var file in Directory.GetFiles(srcPath,
            mod.Name.ToLower() + "_*.txt",
            new EnumerationOptions() { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive }))
            {
                var language = Path.GetFileNameWithoutExtension(file).Replace(mod.Name.ToLower() + "_", "");
                var dst = dstPath + _mergeName.Substring(0, _mergeName.Length - 4) + "_" + language + ".txt";
                Directory.CreateDirectory(dstPath);

                var writer = File.AppendText(dst);
                writer.Write(File.ReadAllText(file));
                writer.Close();

                Console.WriteLine("          Appending " + mod.Name.ToLower() + "_" + "language to " + _mergeName.Substring(0, _mergeName.Length - 4) + "_" + language);


            };
        }

        private static void CopyActorAssets(string dir, string _path, ModKey mod)
        {
            var path = _path.Replace("\\", "/");

            var srcPath = Path.Combine(dir, path, mod.FileName.String.ToLower());
            var dstPath = Path.Combine(_outputDir, path, _mergeName);
            Directory.CreateDirectory(dstPath);

            if (!Directory.Exists(srcPath)) return;

            Console.WriteLine("          Copying assets from directory \"" + Path.Combine(path, mod.FileName.String.ToLower()) + "\"");
            Console.WriteLine("          Copying assets to directory \"" + Path.Combine(path, _mergeName) + "\"");


            _mergeState.Mapping.Where(x => x.Key.ModKey == mod).ForEach(x =>
            {
                var srcId = x.Key.ID;
                var srcIdString = x.Key.IDString().ToLower();

                foreach (var file in Directory.GetFiles(srcPath,
                "*" + srcIdString + "*",
                new EnumerationOptions() { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive }))
                {
                    if (_mergeState.Mapping.Select(x => x.Key.ModKey).Contains(mod) && _mergeState.Mapping.Select(x => x.Key.ID).Contains(srcId))
                    {
                        var newId = "00" + _mergeState.Mapping[new FormKey(mod, srcId)].IDString().ToLower();
                        var dstFile = file.Replace(srcIdString, newId).Replace(srcPath, dstPath);
                        Directory.CreateDirectory(Path.GetDirectoryName(dstFile) ?? "");
                        Console.WriteLine("            Asset renumbered from " + srcIdString + " to " + newId);
                        Console.WriteLine("            Copying asset \"" + file.Replace(srcPath + "/", "") + "\" to \"" + dstFile.Replace(dstPath + "/", "") + "\"");
                        File.Copy(file, dstFile);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(file.Replace(srcPath, dstPath)) ?? "");
                        Console.WriteLine("            Asset not renumbered.");
                        Console.WriteLine("            Copying asset \"" + file.Replace(srcPath + "/", "") + "\"");
                        File.Copy(file, file.Replace(srcPath, dstPath));

                    }

                }


            });
        }

        private static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
