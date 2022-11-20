using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using MutagenMerger.Lib.DI.GameSpecifications;
using Noggog;

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
            env.LoadOrder.ForEach(x => Console.WriteLine(x.Value.ModKey));

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
