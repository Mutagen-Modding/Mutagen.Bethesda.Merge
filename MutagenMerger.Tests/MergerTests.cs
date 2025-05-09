using System.IO.Abstractions;
using Autofac;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using MutagenMerger.Lib;
using MutagenMerger.Lib.DI;
using Noggog;
using Noggog.Testing.Extensions;
using Shouldly;
using Xunit;

namespace MutagenMerger.Tests;

public class MergerTests
{
    public MergerTests()
    {
        Warmup.Init();
    }

    public class TestModule : Module
    {
        private readonly IFileSystem _fileSystem;

        public TestModule(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_fileSystem).As<IFileSystem>()
                .SingleInstance();
            builder.RegisterModule<MergerModule>();
        }
    }
        
    [Theory, MutagenAutoData]
    public void TestMerging(
        IFileSystem fileSystem,
        DirectoryPath existingFolder,
        MutagenTestHelpers testHelpers,
        ModKey modKey1,
        ModKey modKey2,
        ModKey modKey3,
        string editorId1,
        string editorId2,
        string editorId3,
        string editorId4,
        string modifiedEditorId,
        ModKey outputModKey,
        TestModule testModule)
    {
        var mod1 = testHelpers.CreateDummyPlugin(existingFolder, modKey1, mod =>
        {
            mod.Actions.AddNew(editorId1);
            mod.Actions.AddNew(editorId2);
        });

        var mod2 = testHelpers.CreateDummyPlugin(existingFolder, modKey2, mod =>
        {
            mod.Actions.AddNew(editorId3);
            mod.Actions.AddNew(editorId4);
        });

        var mods = new List<ModKey>
        {
            mod1,
            mod2,
        };

        using (var testMod1 = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
                   .FromPath(Path.Combine(existingFolder, mod1))
                   .WithFileSystem(fileSystem)
                   .Construct())
        {
            var action1 = testMod1.Actions.First();

            var mod3 = testHelpers.CreateDummyPlugin(existingFolder, modKey3, mod =>
            {
                var copy = action1.DeepCopy();
                copy.EditorID = modifiedEditorId;
                mod.Actions.Add(copy);
            });
                
            mods.Add(mod3);
        }

        var container = new ContainerBuilder();
        container.RegisterModule(testModule);
        var builder = container.Build();
        var sut = builder.Resolve<Merger<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>>();
        
        sut.Merge(
            dataFolderPath: existingFolder,
            modsToMerge: mods,
            outputKey: outputModKey,
            outputFolder: existingFolder,
            game: GameRelease.SkyrimSE);

        var outputFile = Path.Combine(existingFolder, outputModKey.FileName);
        testHelpers.TestPlugin(outputFile, mod =>
        {
            mod.Actions.Count.ShouldEqual(4);
                
            mod.Actions.ShouldContain(x => x.EditorID == modifiedEditorId);
            mod.Actions.ShouldContain(x => x.EditorID == editorId2);
            mod.Actions.ShouldContain(x => x.EditorID == editorId3);
            mod.Actions.ShouldContain(x => x.EditorID == editorId4);
        });
    }

    [Fact]
    public void TestBasePluginsMerging()
    {
        //requires manual activation
        const string folder = "base-plugins";
        if (!Directory.Exists(folder)) return;

        var mods = new List<ModKey>
        {
            "Skyrim.esm",
            "Update.esm",
            "Dawnguard.esm",
            "HearthFires.esm",
            "Dragonborn.esm"
        };

        if (!mods.All(x => File.Exists(Path.Combine(folder, x.FileName))))
            return;

        const string outputMod = "output.esp";
        var outputPath = Path.Combine(folder, outputMod);
        if (File.Exists(outputPath))
            File.Delete(outputPath);
            
        throw new NotImplementedException();
        // using (var merger = new Merger(folder, mods, mods, outputMod, folder, GameRelease.SkyrimSE))
        // {
        //     merger.Merge();
        // }
            
        Assert.True(File.Exists(outputPath));
    }
}
