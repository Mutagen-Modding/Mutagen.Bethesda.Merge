using System;
using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace MutagenMerger.Tests
{
    public static class MutagenTestHelpers
    {
        public static string CreateDummyPlugin(string folder, string fileName, Action<SkyrimMod> addRecords)
        {
            Directory.CreateDirectory(folder);
            var outputPath = Path.Combine(folder, fileName);
            
            var mod = new SkyrimMod(ModKey.FromNameAndExtension(fileName), SkyrimRelease.SkyrimSE);
            addRecords(mod);
            mod.WriteToBinary(outputPath, new BinaryWriteParameters
            {
                MasterFlag = BinaryWriteParameters.MasterFlagOption.ExceptionOnMismatch,
                RecordCount = BinaryWriteParameters.RecordCountOption.Iterate
            });
            return fileName;
        }

        public static void TestPlugin(string path, Action<ISkyrimModDisposableGetter> verify)
        {
            Assert.True(File.Exists(path));

            using var mod = SkyrimMod.CreateFromBinaryOverlay(ModPath.FromPath(path), SkyrimRelease.SkyrimSE);
            verify(mod);
        }
    }
}
