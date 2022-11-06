using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace MutagenMerger.Lib
{
    public interface IMerger
    {
        void Merge(
            DirectoryPath dataFolderPath,
            List<ModKey> plugins,
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
        public void Merge(
            DirectoryPath dataFolderPath,
            List<ModKey> plugins,
            IEnumerable<ModKey> modsToMerge, 
            ModKey outputKey,
            DirectoryPath outputFolder,
            GameRelease game)
        {
            using var loadOrder = LoadOrder.Import(
                dataFolderPath,
                plugins,
                path => ModInstantiator<TModGetter>.Importer(path, game));
            
            var outputMod = ModInstantiator.Activator(outputKey, game) as TMod ?? throw new Exception("Could not instantiate mod");

            var mods = loadOrder.PriorityOrder.Resolve().ToArray();
            var mergingMods = mods.Where(x => modsToMerge.Contains(x.ModKey)).ToArray();

            var outputFile = Path.Combine(outputFolder, outputKey.FileName);

            Console.WriteLine("Merging " + String.Join(", ",mergingMods.Select(x => x.ModKey.FileName.String)) + " into " + Path.GetFileName(outputFile));
            Console.WriteLine();
            
            var modsToMergeSet = modsToMerge.ToHashSet();

            var linkCache = mods.ToImmutableLinkCache<TMod, TModGetter>();

            var state = new MergeState<TMod, TModGetter>(
                game,
                mods,
                modsToMergeSet,
                outputMod,
                OutputPath: outputFile,
                DataPath: dataFolderPath,
                LinkCache: linkCache);
            
            Merge<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>.DoMerge(state);
        }
    }
}
