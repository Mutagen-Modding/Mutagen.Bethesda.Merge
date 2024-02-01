using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Json;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using MutagenMerger.Lib.DI.GameSpecifications;
using Noggog;
using System.Security.Cryptography;

namespace MutagenMerger.Lib.DI
{
    public interface IMerger
    {
        void Merge(
            DirectoryPath dataFolderPath,
            IEnumerable<ModKey> modsToMerge, 
            ModKey outputKey,
            DirectoryPath outputFolder,
            GameRelease game);
    }

    public sealed class Merger<TModGetter, TMod, TMajorRecord, TMajorRecordGetter> : IMerger
        where TModGetter : class, IModGetter, IContextGetterMod<TMod, TModGetter>
        where TMod : class, TModGetter, IMod, IContextMod<TMod, TModGetter>
        where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        private readonly IGameSpecifications<TModGetter, TMod, TMajorRecord, TMajorRecordGetter> _gameSpecs;
        private readonly CopyRecordProcessor<TMod, TModGetter> _copyRecordProcessor;

        public Merger(
            IGameSpecifications<TModGetter, TMod, TMajorRecord, TMajorRecordGetter> gameSpecs,
            CopyRecordProcessor<TMod, TModGetter> copyRecordProcessor)
        {
            _gameSpecs = gameSpecs;
            _copyRecordProcessor = copyRecordProcessor;
        }
        
        public void Merge(
            DirectoryPath dataFolderPath,
            IEnumerable<ModKey> modsToMerge, 
            ModKey outputKey,
            DirectoryPath outputFolder,
            GameRelease game)
        {
            var outputMod = ModInstantiator.Activator(outputKey, game) as TMod ?? throw new Exception("Could not instantiate mod");
            var env = GameEnvironmentBuilder<TMod,TModGetter>.Create(game).WithTargetDataFolder(dataFolderPath).WithOutputMod(outputMod).Build();
            var mods = env.LoadOrder.PriorityOrder.Resolve().ToArray();
            var mergingMods = mods.Where(x => modsToMerge.Contains(x.ModKey)).ToArray();

            var outputFile = Path.Combine(outputFolder, outputKey.FileName);
            // env.LoadOrder.ForEach(x => Console.WriteLine(x.Value.ModKey));

            Console.WriteLine("Merging " + String.Join(", ",mergingMods.Select(x => x.ModKey.FileName.String)) + " into " + Path.GetFileName(outputFile));
            Console.WriteLine();
            
            var modsToMergeSet = modsToMerge.ToHashSet();

            var linkCache = mergingMods.ToImmutableLinkCache<TMod, TModGetter>();

            var state = new MergeState<TMod, TModGetter>(
                game,
                mods,
                modsToMergeSet,
                outputMod,
                OutputPath: outputFile,
                DataPath: dataFolderPath,
                LinkCache: linkCache,
                env: env);
            
            _copyRecordProcessor.CopyRecords(state);
            // state.Mapping.ForEach(x => Console.WriteLine(x.Key.ToString() + " " + x.Value.ToString()));
            state.OutgoingMod.RemapLinks(state.Mapping);

            Directory.CreateDirectory(state.OutputPath.Directory ?? "");
            state.OutgoingMod.WriteToBinary(state.OutputPath, Utils.SafeBinaryWriteParameters(env.LoadOrder.Keys));

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
            var _outputDir = Path.GetDirectoryName(state.OutputPath) ?? "";
            var _mergeName = Path.GetFileNameWithoutExtension(state.OutputPath);
            var _mergePlugin = state.OutgoingMod.ModKey.FileName;
            var _mergeDir = Path.Combine(_outputDir, "merge - " + _mergeName);
            if (!Directory.Exists(_mergeDir))
            {
                Directory.CreateDirectory(_mergeDir);
            }

            JsonObject? _mergeJson = new()
            {
                { "name", new JsonPrimitive(_mergeName) },
                { "filename", new JsonPrimitive(_mergePlugin)},
                { "method", new JsonPrimitive("Mutagen.Bethesda.Merge")},
                { "loadOrder", new JsonArray (
                    state.env.LoadOrder.PriorityOrder.Resolve().Select(x => new JsonPrimitive(x.ModKey.FileName)).ToArray()
                )},
                {"dateBuilt", new JsonPrimitive(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.zzZ"))},
                {"plugins", new JsonArray (
                    state.ModsToMerge.Select(x => { MD5 md5 = MD5.Create(); return new JsonObject
                    {
                        { "filename", new JsonPrimitive(x.FileName) },
                        { "hash", new JsonPrimitive(BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(Path.Combine(state.env.DataFolderPath,x.FileName)))).Replace("-", "").ToLowerInvariant()) },
                        { "dataFolder", new JsonPrimitive(state.env.DataFolderPath)}

                    }; }).ToArray()
                )}
            };

            File.WriteAllText(Path.Combine(_mergeDir, "merge.json"), _mergeJson.ToString());

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

            File.WriteAllText(Path.Combine(_mergeDir, "map.json"), _mapJson.ToString());



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

            File.WriteAllText(Path.Combine(_mergeDir, "fidCache.json"), _fidJson.ToString());

        }

        private void HandleScripts(
            MergeState<TMod, TModGetter> mergeState)
        {
            // Console.WriteLine("HandleScript");
        }

        private void HandleAssets(MergeState<TMod, TModGetter> mergeState)
        {
            AssetMerge<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>.Handle(mergeState);
        }
    }
}
