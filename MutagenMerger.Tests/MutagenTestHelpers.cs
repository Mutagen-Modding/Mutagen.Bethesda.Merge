using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Shouldly;
using Xunit;

namespace MutagenMerger.Tests;

public class MutagenTestHelpers
{
    private readonly IFileSystem _fileSystem;

    public MutagenTestHelpers(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public ModPath CreateDummyPlugin(DirectoryPath directory, ModKey modName, Action<SkyrimMod> addRecords)
    {
        var modPath = ModPath.FromPath(Path.Combine(directory, modName.FileName));
        modPath.Path.Directory?.Create(_fileSystem);
            
        var mod = new SkyrimMod(modPath.ModKey, SkyrimRelease.SkyrimSE);
        addRecords(mod);
        mod.BeginWrite
            .ToPath(modPath.Path)
            .WithNoLoadOrder()
            .WithFileSystem(_fileSystem)
            .Write();

        return modPath;
    }

    public void TestPlugin(ModPath path, Action<ISkyrimModDisposableGetter> verify)
    {
        _fileSystem.File.Exists(path).ShouldBeTrue();

        using var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(path)
            .WithFileSystem(_fileSystem)
            .Construct();
        verify(mod);
    }
}
