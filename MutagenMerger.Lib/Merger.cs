using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        public Merger(string dataFolderPath, List<string> plugins, string outputName)
        {
            //TODO: see https://discord.com/channels/759302581448474626/759344198792380416/821399241200238662
            //WarmupSkyrim.Init();

            _loadOrder = LoadOrder.Import(
                dataFolderPath,
                plugins.Select(x => ModKey.FromNameAndExtension(x)), 
                path => ModInstantiator<ISkyrimModGetter>.Importer(path, GameRelease.SkyrimSE));

            _outputMod = new SkyrimMod(ModKey.FromNameAndExtension(outputName), SkyrimRelease.SkyrimSE);
            _outputPath = Path.Combine(dataFolderPath, outputName);
        }
        
        public void Merge()
        {
            var linkCache = _loadOrder.ToImmutableLinkCache();
            
            foreach (var listing in _loadOrder.PriorityOrder)
            {
                if (listing.Mod == null) continue;
                if (!listing.Enabled) continue;
                
                foreach (var rec in listing.Mod
                    .EnumerateMajorRecordContexts<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(linkCache))
                {
                    rec.DuplicateIntoAsNewRecord(_outputMod, rec.Record.EditorID);
                }
            }
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
