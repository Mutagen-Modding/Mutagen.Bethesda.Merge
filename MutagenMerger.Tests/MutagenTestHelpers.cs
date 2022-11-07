using System;
using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using MutagenMerger.Lib;
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
            mod.WriteToBinary(outputPath, Utils.SafeBinaryWriteParameters);
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
