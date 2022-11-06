using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;

namespace MutagenMerger.Lib
{
    [PublicAPI]
    public sealed class Merger<TModGetter, TMod, TMajorRecord, TMajorRecordGetter> : IDisposable
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>, IMajorRecordGetterEnumerable, IContextGetterMod<TMod, TModGetter>
        where TMod : class, TModGetter, IMod, IContextMod<TMod, TModGetter>
        where TMajorRecord : class, TMajorRecordGetter, IMajorRecord
        where TMajorRecordGetter : class, IMajorRecordGetter
    {
        private readonly ILoadOrder<IModListing<TModGetter>> _loadOrder;
        private readonly TMod _outputMod;
        private readonly string _outputPath;
        private readonly IEnumerable<ModKey> _plugins;
        private readonly List<ModKey> _modsToMerge;
        private readonly GameRelease _game;
        private readonly string _dataFolderPath;

        public Merger(string dataFolderPath, List<ModKey> plugins, List<ModKey> modsToMerge, ModKey outputKey, string outputFolder, GameRelease game)
        {
            _game = game;
            _loadOrder = LoadOrder.Import(
                dataFolderPath,
                plugins,
                path => ModInstantiator<TModGetter>.Importer(path, _game));

            _outputMod = ModInstantiator.Activator(outputKey, _game) as TMod ?? throw new ArgumentNullException(nameof(_outputMod));

            _outputPath = Path.Combine(outputFolder, outputKey.FileName);
            _dataFolderPath = dataFolderPath;
            _plugins = plugins;
            _modsToMerge = modsToMerge;
        }
        public void Merge()
        {
            var mods = _loadOrder.PriorityOrder.Resolve();
            var mergingMods = mods.Where(x => _modsToMerge.Contains(x.ModKey)).ToArray();

            Console.WriteLine("Merging " + String.Join(", ",mergingMods.Select(x => x.ModKey.FileName.String)) + " into " + Path.GetFileName(_outputPath));
            Console.WriteLine();

            mergingMods.MergeMods<TModGetter, TMod, TMajorRecord, TMajorRecordGetter>(
                _outputMod, _game, _dataFolderPath, _outputPath,
                out var mapping);
        }

        public void Dispose()
        {
            _loadOrder.Dispose();
        }
    }
}
