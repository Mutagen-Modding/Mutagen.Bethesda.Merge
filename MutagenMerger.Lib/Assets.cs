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

        static Dictionary<FormKey, FormKey>? _mapping;
        static string _outputDir = "";
        static string _mergeName = "";
        static GameRelease _game;
        private static HashSet<ModKey>? _modsToMerge;
        static string temp = GetTemporaryDirectory();
        public static List<string> Rules
        {
            get
            {
                var rules = new List<string>() {"**/*.@(esp|esm|bsa|ba2|bsl)", "meta.ini",
                    "interface/translations/*.txt", "TES5Edit Backups/**/*",
                    "fomod/**/*", "screenshot?(s)/**/*", "scripts/source/*.psc"};

                _modsToMerge!.ForEach(x =>
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

        public static void Handle(TMod outputMod, Dictionary<FormKey, FormKey> mapping, TModGetter[] modsCached, HashSet<ModKey> modsToMerge, ImmutableLoadOrderLinkCache<TMod, TModGetter> linkCache, GameRelease game, string dataPath, string _outputPath)
        {
            _mapping = mapping;
            _outputDir = Path.GetDirectoryName(_outputPath) ?? "";
            _mergeName = Path.GetFileName(_outputPath);
            _game = game;
            _modsToMerge = modsToMerge;
            var matcher = new Matcher();
            matcher.AddIncludePatterns(new string[] { "**/*" });
            matcher.AddExcludePatterns(Rules);

            foreach (var mod in modsToMerge)
            {

                var bsaPattern = mod.FileName.NameWithoutExtension + "*." + (game == GameRelease.Fallout4 ? "b2a" : "bsa");
                string[] bsaFiles = Directory.GetFiles(dataPath, bsaPattern);

                // bsaFiles.ForEach(Console.WriteLine);

                foreach (string bsa in bsaFiles)
                {
                    ExtractBSA(bsa);
                }
                Console.WriteLine();

                // // //   // copy loose File/FormID specific assets
                // CopyActorAssets(dataPath, paths[0], mod);
                // CopyActorAssets(dataPath, paths[1], mod);
                // CopyVoiceAssets(dataPath, paths[2], mod);
                // CopyTranslations(dataPath, paths[3], mod);


                // // //   // copy archived File/FormID specific assets
                // CopyActorAssets(temp, paths[0], mod);
                // CopyActorAssets(temp, paths[1], mod);
                // CopyVoiceAssets(temp, paths[2], mod);
                // CopyTranslations(temp, paths[3], mod);

            }

            var matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(temp)));

            matches.Files.ForEach(x =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(_outputDir, x.Path)) ?? "");
                Console.WriteLine("          Copying extracted asset \"" + x.Path + "\"");
                File.Copy(Path.Combine(temp, x.Path), Path.Combine(_outputDir, x.Path));
            });

            foreach (var mod in modsToMerge)
            {
                CopyAssets(dataPath, mod);
                CopyAssets(temp, mod);
            }

            BuildSeqFile(dataPath, temp, outputMod);
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
            var masterColl = new MasterReferenceCollection(merge.ModKey, merge.MasterReferences);

            IGroup quests = _game switch
            {
                GameRelease.Oblivion => merge.GetTopLevelGroup<Oblivion.Quest>(),
                GameRelease.Fallout4 => merge.GetTopLevelGroup<Fallout4.Quest>(),
                _ => merge.GetTopLevelGroup<Skyrim.Quest>(),
            };
            // quests.Records.ForEach(x => Console.WriteLine(x.EditorID));
            // quests.Records.ForEach(x =>
            // {
            //     Console.WriteLine(_game switch
            //     {
            //         GameRelease.Oblivion => String.Join(", ", Enum.GetValues<Oblivion.Quest.Flag>().Where(e => ((Oblivion.Quest)x).Data!.Flags.HasFlag(e))),
            //         GameRelease.Fallout4 => String.Join(", ", Enum.GetValues<Fallout4.Quest.Flag>().Where(e => ((Fallout4.Quest)x).Data!.Flags.HasFlag(e))),
            //         _ => String.Join(", ", Enum.GetValues<Skyrim.Quest.Flag>().Where(e => ((Skyrim.Quest)x).Flags.HasFlag(e))),
            //     });
            // });

            // quests.Records.Where(x => ((Skyrim.Quest)x).Flags.HasFlag(Skyrim.Quest.Flag.StartGameEnabled))
            //               .Select(x => x.FormKey.IDString())
            //               .ForEach(Console.WriteLine);

            var masterCount = merge.MasterReferences.Count;
            List<UInt32> formIds = new List<UInt32>();

            if (quests.Count == 0) return formIds;

            

            foreach (var quest in quests.Records)
            {
                var fid = (uint)(masterColl.GetFormID(quest.FormKey).ID + 0x01000000 * masterCount);
                formIds.Add(fid);
            }
            
                merge.MasterReferences.Select(x => x.Master.FileName.String).ForEach(x=> Console.WriteLine(x));
                masterColl.Masters.Select(x => x.Master.FileName.String).ForEach(x=> Console.WriteLine(x));
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

            var reader = Archive.CreateReader(_game, bsa);
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


            _mapping!.Where(x => x.Key.ModKey == mod).ForEach(x =>
            {
                var srcId = x.Key.ID;
                var srcIdString = x.Key.IDString().ToLower();

                foreach (var file in Directory.GetFiles(srcPath,
                "*" + srcIdString + "*",
                new EnumerationOptions() { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive }))
                {
                    if (_mapping!.Select(x => x.Key.ModKey).Contains(mod) && _mapping!.Select(x => x.Key.ID).Contains(srcId))
                    {
                        var newId = "00" + _mapping![new FormKey(mod, srcId)].IDString().ToLower();
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