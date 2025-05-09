using System.IO.Abstractions;
using System.Json;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.Security.Cryptography;
using Mutagen.Bethesda.Environments.DI;

namespace MutagenMerger.Lib.DI;

public interface IMerger
{
    void Merge(
        IEnumerable<ModKey> modsToMerge, 
        ModKey outputKey,
        DirectoryPath outputFolder);
}

public sealed class Merger<TMod, TModGetter, TMajorRecord, TMajorRecordGetter> : IMerger
    where TMod : class, TModGetter, IMod, IContextMod<TMod, TModGetter>
    where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
    where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
    where TMajorRecordGetter : class, IMajorRecordGetter
{
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly AssetMerge<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>.Factory _assetMergeFactory;
    private readonly CopyRecordProcessor<TMod, TModGetter> _copyRecordProcessor;

    public Merger(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IGameReleaseContext gameReleaseContext,
        AssetMerge<TMod, TModGetter, TMajorRecord, TMajorRecordGetter>.Factory assetMergeFactory,
        CopyRecordProcessor<TMod, TModGetter> copyRecordProcessor)
    {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
        _gameReleaseContext = gameReleaseContext;
        _assetMergeFactory = assetMergeFactory;
        _copyRecordProcessor = copyRecordProcessor;
    }
        
    public void Merge(
        IEnumerable<ModKey> modsToMerge, 
        ModKey outputKey,
        DirectoryPath outputFolder)
    {
        var outputMod = ModInstantiator<TMod>.Activator(outputKey, _gameReleaseContext.Release);
        var env = GameEnvironmentBuilder<TMod,TModGetter>
            .Create(_gameReleaseContext.Release)
            .WithTargetDataFolder(_dataDirectoryProvider.Path)
            .WithOutputMod(outputMod)
            .WithFileSystem(_fileSystem)
            .Build();
        var mods = env.LoadOrder.PriorityOrder.ResolveAllModsExist().ToArray();
        var mergingMods = mods.Where(x => modsToMerge.Contains(x.ModKey)).ToArray();

        var outputFile = Path.Combine(outputFolder, outputKey.FileName);
        // env.LoadOrder.ForEach(x => Console.WriteLine(x.Value.ModKey));

        Console.WriteLine("Merging " + String.Join(", ",mergingMods.Select(x => x.ModKey.FileName.String)) + " into " + Path.GetFileName(outputFile));
        Console.WriteLine();
            
        var modsToMergeSet = modsToMerge.ToHashSet();

        var linkCache = mergingMods.ToImmutableLinkCache<TMod, TModGetter>();

        var state = new MergeState<TMod, TModGetter>(
            _gameReleaseContext.Release,
            mods,
            modsToMergeSet,
            outputMod,
            OutputPath: outputFile,
            DataPath: _dataDirectoryProvider.Path,
            LinkCache: linkCache,
            env: env);
            
        _copyRecordProcessor.CopyRecords(state);
        // state.Mapping.ForEach(x => Console.WriteLine(x.Key.ToString() + " " + x.Value.ToString()));
        state.OutgoingMod.RemapLinks(state.Mapping);

        _fileSystem.Directory.CreateDirectory(state.OutputPath.Directory ?? "");
        state.OutgoingMod.BeginWrite
            .ToPath(state.OutputPath)
            .WithLoadOrder(env.LoadOrder.Keys)
            .WithDataFolder(env.DataFolderPath)
            .WithFileSystem(_fileSystem)
            .Write();

        // foreach (var rec in state.OutgoingMod.EnumerateMajorRecords())
        // {
        //     foreach (var link in rec.EnumerateFormLinks())
        //     {
        //         IMajorRecordGetter? majorRecord = null;
        //         link.TryResolveCommon(state.LinkCache, out majorRecord);
        //         Console.WriteLine(majorRecord?.ToString() + ":" +link.FormKey);
        //     }
        // }

        HandleAssets(state);

        HandleScripts(state);

        MergeJson(state);
    }

    private void MergeJson(MergeState<TMod, TModGetter> state)
    {
        var outputDir = Path.GetDirectoryName(state.OutputPath) ?? "";
        var mergeName = Path.GetFileNameWithoutExtension(state.OutputPath);
        var mergePlugin = state.OutgoingMod.ModKey.FileName;
        var mergeDir = Path.Combine(outputDir, "merge - " + mergeName);
        if (!_fileSystem.Directory.Exists(mergeDir))
        {
            _fileSystem.Directory.CreateDirectory(mergeDir);
        }

        JsonObject? _mergeJson = new()
        {
            { "name", new JsonPrimitive(mergeName) },
            { "filename", new JsonPrimitive(mergePlugin)},
            { "method", new JsonPrimitive("Mutagen.Bethesda.Merge")},
            { "loadOrder", new JsonArray (
                state.env.LoadOrder.PriorityOrder.Resolve().Select(x => new JsonPrimitive(x.ModKey.FileName)).ToArray()
            )},
            {"dateBuilt", new JsonPrimitive(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.zzZ"))},
            {"plugins", new JsonArray (
                state.ModsToMerge.Select(x => { MD5 md5 = MD5.Create(); return new JsonObject
                {
                    { "filename", new JsonPrimitive(x.FileName) },
                    { "hash", new JsonPrimitive(BitConverter.ToString(md5.ComputeHash(_fileSystem.File.ReadAllBytes(Path.Combine(state.env.DataFolderPath,x.FileName)))).Replace("-", "").ToLowerInvariant()) },
                    { "dataFolder", new JsonPrimitive(state.env.DataFolderPath)}

                }; }).ToArray()
            )}
        };

        _fileSystem.File.WriteAllText(Path.Combine(mergeDir, "merge.json"), _mergeJson.ToString());

        JsonObject? _mapJson = new(
            state.ModsToMerge.Select(
                x => new KeyValuePair<string, JsonValue>(
                    x.FileName,
                    new JsonObject(state.Mapping.Where(y => y.Key.ModKey.Equals(x) && y.Key.ID != y.Value.ID)
                        .Select(
                            y => new KeyValuePair<string, JsonValue>(
                                y.Key.IDString(), 
                                new JsonPrimitive(y.Value.IDString())
                            )
                        ).ToArray()))
            )
        );

        _fileSystem.File.WriteAllText(Path.Combine(mergeDir, "map.json"), _mapJson.ToString());



        JsonObject? _fidJson = new(
            state.ModsToMerge.Select(
                x => new KeyValuePair<string, JsonValue>(
                    x.FileName,
                    new JsonArray(state.Mapping.Where(y => y.Key.ModKey.Equals(x))
                        .Select(
                            y =>  new JsonPrimitive(y.Value.IDString())
                        ).ToArray()))
            )
        );

        _fileSystem.File.WriteAllText(Path.Combine(mergeDir, "fidCache.json"), _fidJson.ToString());

    }

    private void HandleScripts(
        MergeState<TMod, TModGetter> mergeState)
    {
        // Console.WriteLine("HandleScript");
    }

    private void HandleAssets(MergeState<TMod, TModGetter> mergeState)
    {
        var assetMerge = _assetMergeFactory(mergeState);
        assetMerge.Handle();
    }
}
