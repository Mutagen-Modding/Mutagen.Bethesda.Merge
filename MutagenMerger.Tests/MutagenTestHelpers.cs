using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace MutagenMerger.Tests;

public class MutagenTestHelpers
{
    private readonly IFileSystem _fileSystem;

    public MutagenTestHelpers(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public string CreateDummyPlugin(string folder, string fileName, Action<SkyrimMod> addRecords)
    {
        _fileSystem.Directory.CreateDirectory(folder);
        var outputPath = Path.Combine(folder, fileName);
            
        var mod = new SkyrimMod(ModKey.FromNameAndExtension(fileName), SkyrimRelease.SkyrimSE);
        addRecords(mod);
        mod.BeginWrite
            .ToPath(outputPath)
            .WithNoLoadOrder()
            .WithFileSystem(_fileSystem)
            .Write();
        return fileName;
    }

    public void TestPlugin(string path, Action<ISkyrimModDisposableGetter> verify)
    {
        Assert.True(_fileSystem.File.Exists(path));

        using var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(path)
            .WithFileSystem(_fileSystem)
            .Construct();
        verify(mod);
    }
}
