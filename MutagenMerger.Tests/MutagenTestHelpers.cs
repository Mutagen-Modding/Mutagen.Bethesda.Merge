using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.Fakes;
using Noggog;
using Shouldly;

namespace MutagenMerger.Tests;

public class MutagenTestHelpers
{
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public MutagenTestHelpers(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider)
    {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
    }
    
    public ModPath CreateDummyPlugin(ModKey modName, Action<SkyrimMod> addRecords)
    {
        var modPath = ModPath.FromPath(Path.Combine(_dataDirectoryProvider.Path, modName.FileName));
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
