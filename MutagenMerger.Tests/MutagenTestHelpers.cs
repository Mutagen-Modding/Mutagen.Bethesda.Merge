using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using MutagenMerger.Lib;
using Xunit;

namespace MutagenMerger.Tests;

public static class MutagenTestHelpers
{
    public static string CreateDummyPlugin(string folder, string fileName, Action<SkyrimMod> addRecords)
    {
        Directory.CreateDirectory(folder);
        var outputPath = Path.Combine(folder, fileName);
            
        var mod = new SkyrimMod(ModKey.FromNameAndExtension(fileName), SkyrimRelease.SkyrimSE);
        addRecords(mod);
        mod.BeginWrite
            .ToPath(outputPath)
            .WithNoLoadOrder()
            .Write();
        return fileName;
    }

    public static void TestPlugin(string path, Action<ISkyrimModDisposableGetter> verify)
    {
        Assert.True(File.Exists(path));

        using var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(path)
            .Construct();
        verify(mod);
    }
}
