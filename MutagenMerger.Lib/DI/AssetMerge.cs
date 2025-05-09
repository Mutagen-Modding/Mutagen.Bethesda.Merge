using System.IO.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Noggog.IO;
using Skyrim = Mutagen.Bethesda.Skyrim;
using Fallout4 = Mutagen.Bethesda.Fallout4;
using Oblivion = Mutagen.Bethesda.Oblivion;
using DirectoryInfoWrapper = Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper;

namespace MutagenMerger.Lib.DI;

public class AssetMerge<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>
    where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
    where TMod : class, IMod, IContextMod<TMod, TModGetter>, TModGetter
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter
{
    private readonly IFileSystem _fileSystem;
    private readonly MergeState<TMod, TModGetter> _mergeState;
    private readonly string _outputDir;
    private readonly string _mergeName;
    private readonly List<string> _rules;

    public delegate AssetMerge<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> Factory(
        MergeState<TMod, TModGetter> mergeState);
    
    public AssetMerge(
        MergeState<TMod, TModGetter> mergeState,
        IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _mergeState = mergeState;
        _rules = GetRules();
        _outputDir = Path.GetDirectoryName(_mergeState.OutputPath) ?? "";
        _mergeName = Path.GetFileName(_mergeState.OutputPath);
    }

    private List<string> GetRules()
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
    
    public void Handle()
    {
        using var temp = TempFolder.Factory();
        var matcher = new Matcher();
        matcher.AddIncludePatterns(new string[] { "**/*" });
        matcher.AddExcludePatterns(_rules);
        Parallel.ForEach(_mergeState.ModsToMerge, mod => {
            var bsaPattern = mod.FileName.NameWithoutExtension + "*." + (_mergeState.Release == GameRelease.Fallout4 ? "b2a" : "bsa");
            string[] bsaFiles = _fileSystem.Directory.GetFiles(_mergeState.DataPath, bsaPattern);

            // bsaFiles.ForEach(Console.WriteLine);

            foreach (string bsa in bsaFiles)
            {
                ExtractBSA(bsa, temp.Dir);
            }
            // Console.WriteLine();
        });

        var matches = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(temp.Dir)));

        Parallel.ForEach(matches.Files, file => {
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(_outputDir, file.Path)) ?? "");
            Console.WriteLine("            Copying extracted asset \"" + file.Path + "\"");
            _fileSystem.File.Copy(Path.Combine(temp.Dir, file.Path), Path.Combine(_outputDir, file.Path));
        });

        foreach (var mod in _mergeState.ModsToMerge)
        {
            CopyAssets(_mergeState.DataPath, mod);
            CopyAssets(temp.Dir, mod);
        }

        BuildSeqFile(_mergeState.DataPath, temp.Dir, _mergeState.OutgoingMod);
    }

    private void BuildSeqFile(string dataPath, DirectoryPath temp, TMod outputMod)
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
        _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? "");
        _fileSystem.File.WriteAllBytes(filePath, buffer);
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

    private List<UInt32> GetSeqQuests(TMod merge)
    {
        var masterColl = MasterReferenceCollection.FromPath(
            Path.Combine(_outputDir, _mergeName), 
            _mergeState.Release,
            _fileSystem);

        IGroup quests = _mergeState.Release switch
        {
            GameRelease.Oblivion => merge.GetTopLevelGroup<Oblivion.Quest>(),
            GameRelease.Fallout4 => merge.GetTopLevelGroup<Fallout4.Quest>(),
            _ => merge.GetTopLevelGroup<Skyrim.Quest>(),
        };

        List<UInt32> formIds = new List<UInt32>();

        if (quests.Count == 0) return formIds;

        Parallel.ForEach(quests.Records, quest => {
            bool startGameEnabled = _mergeState.Release switch {
                GameRelease.Oblivion => ((Oblivion.Quest)quest).Data?.Flags.HasFlag(Oblivion.Quest.Flag.StartGameEnabled) ?? false,
                GameRelease.Fallout4 => ((Fallout4.Quest)quest).Data?.Flags.HasFlag(Fallout4.Quest.Flag.StartGameEnabled) ?? false,
                _ => ((Skyrim.Quest)quest).Flags.HasFlag(Skyrim.Quest.Flag.StartGameEnabled)
            };
            if (startGameEnabled)
            {
                throw new NotImplementedException();
                // This got hidden in latest mutagen after Starfield flipped the table.
                // Will need to re-expose how FormIDs are generated in a tool the public has access to.
                // var fid = masterColl.GetFormID(quest.FormKey).Raw;
                // formIds.Add(fid);
            }
        });

        return formIds;
    }

    private void CopyAssets(DirectoryPath path, ModKey mod)
    {
        CopyActorAssets(path, "textures/actors/character/facegendata/facetint", mod);
        CopyActorAssets(path, "meshes/actors/character/facegendata/facegeom", mod);
        CopyActorAssets(path, "sound/voice", mod);

        CopyTranslations(path, mod);
    }

    private void ExtractBSA(string bsa, DirectoryPath temp)
    {
        Console.WriteLine();

        var reader = Archive.CreateReader(_mergeState.Release, bsa);
        var files = reader.Files.ToArray();
        Parallel.For(0,files.Count(), i => {
            var file = files[i];
            var filePath = file.Path.Replace("\\", "/").ToLower();
            _fileSystem.Directory.CreateDirectory(Path.Combine(temp, Path.GetDirectoryName(filePath) ?? ""));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("          Extracting Archive \"" + Path.GetFileName(bsa) + "\" " + ((decimal)i / files.Count()).ToString("0.00%"));
            File.WriteAllBytes(Path.Combine(temp, filePath), file.GetBytes());
        });
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write("          Extracting Archive \"" + Path.GetFileName(bsa) + "\" 100.00%");
    }

    private void CopyTranslations(string dir, ModKey mod)
    {
        var path = "interface/translations/";
        var srcPath = Path.Combine(dir, path);
        var dstPath = Path.Combine(_outputDir, path);

        if (!_fileSystem.Directory.Exists(srcPath)) return;

        foreach (var file in _fileSystem.Directory.GetFiles(srcPath,
                     mod.Name.ToLower() + "_*.txt",
                     new EnumerationOptions() { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive }))
        {
            var language = Path.GetFileNameWithoutExtension(file).Replace(mod.Name.ToLower() + "_", "");
            var dst = dstPath + _mergeName.Substring(0, _mergeName.Length - 4) + "_" + language + ".txt";
            _fileSystem.Directory.CreateDirectory(dstPath);

            var writer = _fileSystem.File.AppendText(dst);
            writer.Write(_fileSystem.File.ReadAllText(file));
            writer.Close();

            Console.WriteLine("          Appending " + mod.Name.ToLower() + "_" + "language to " + _mergeName.Substring(0, _mergeName.Length - 4) + "_" + language);


        };
    }

    private void CopyActorAssets(string dir, string _path, ModKey mod)
    {
        var path = _path.Replace("\\", "/");

        var srcPath = Path.Combine(dir, path, mod.FileName.String.ToLower());
        var dstPath = Path.Combine(_outputDir, path, _mergeName);
        _fileSystem.Directory.CreateDirectory(dstPath);

        if (!_fileSystem.Directory.Exists(srcPath)) return;

        Console.WriteLine("          Copying assets from directory \"" + Path.Combine(path, mod.FileName.String.ToLower()) + "\"");
        Console.WriteLine("          Copying assets to directory \"" + Path.Combine(path, _mergeName) + "\"");
        
        _mergeState.Mapping.Where(x => x.Key.ModKey == mod).ForEach(x =>
        {
            var srcId = x.Key.ID;
            var srcIdString = x.Key.IDString().ToLower();

            foreach (var file in _fileSystem.Directory.GetFiles(srcPath,
                         "*" + srcIdString + "*",
                         new EnumerationOptions() { RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive }))
            {
                if (_mergeState.Mapping.Select(x => x.Key.ModKey).Contains(mod) && _mergeState.Mapping.Select(x => x.Key.ID).Contains(srcId))
                {
                    var newId = "00" + _mergeState.Mapping[new FormKey(mod, srcId)].IDString().ToLower();
                    var dstFile = file.Replace(srcIdString, newId).Replace(srcPath, dstPath);
                    _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(dstFile) ?? "");
                    Console.WriteLine("            Asset renumbered from " + srcIdString + " to " + newId);
                    Console.WriteLine("            Copying asset \"" + file.Replace(srcPath + "/", "") + "\" to \"" + dstFile.Replace(dstPath + "/", "") + "\"");
                    _fileSystem.File.Copy(file, dstFile);
                }
                else
                {
                    _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(file.Replace(srcPath, dstPath)) ?? "");
                    Console.WriteLine("            Asset not renumbered.");
                    Console.WriteLine("            Copying asset \"" + file.Replace(srcPath + "/", "") + "\"");
                    _fileSystem.File.Copy(file, file.Replace(srcPath, dstPath));

                }

            }
        });
    }
}
