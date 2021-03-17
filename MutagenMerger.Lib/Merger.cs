using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace MutagenMerger.Lib
{
    [PublicAPI]
    public sealed class Merger : IDisposable
    {
        private readonly LoadOrder<IModListing<ISkyrimModGetter>> _loadOrder;
        private readonly SkyrimMod _outputMod;
        private readonly string _outputPath;
        
        public HashSet<FormKey>? BrokenKeys { get; set; }
        
        public Merger(string dataFolderPath, List<ModKey> plugins, ModKey outputKey)
        {
            _loadOrder = LoadOrder.Import(
                dataFolderPath,
                plugins, 
                path => ModInstantiator<ISkyrimModGetter>.Importer(path, GameRelease.SkyrimSE));

            _outputMod = new SkyrimMod(outputKey, SkyrimRelease.SkyrimSE);
            _outputPath = Path.Combine(dataFolderPath, outputKey.FileName);
        }
        
        public void Merge()
        {
            var linkCache = _loadOrder.ToImmutableLinkCache();

            _loadOrder.MergeMods<ISkyrimModGetter, ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(linkCache,
                _loadOrder.Select(x => x.Key).ToList(), _outputMod, out var brokenKeys);
            BrokenKeys = brokenKeys;
        }

        public void Dispose()
        {
            _loadOrder.Dispose();
            _outputMod.WriteToBinary(_outputPath, new BinaryWriteParameters
            {
                MasterFlag = BinaryWriteParameters.MasterFlagOption.ExceptionOnMismatch
            });
        }
    }
}
