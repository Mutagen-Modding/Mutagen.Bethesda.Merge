using System.IO.Abstractions;
using Autofac;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Testing.Fakes;
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
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<MergerModule>();
            builder.RegisterType<MutagenTestHelpers>().AsSelf();
        }
    }
        
    [Theory, MutagenContainerAutoData<TestModule>(GameRelease.SkyrimSE)]
    public void TestMerging(
        IFileSystem fileSystem,
        ManualLoadOrderProvider loadOrderProvider,
        ManualDataDirectoryProvider dataDirectory,
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
        Merger<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> sut)
    {
        var mod1 = testHelpers.CreateDummyPlugin(modKey1, mod =>
        {
            mod.Actions.AddNew(editorId1);
            mod.Actions.AddNew(editorId2);
        });

        var mod2 = testHelpers.CreateDummyPlugin(modKey2, mod =>
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
                   .FromPath(Path.Combine(dataDirectory.Path, mod1))
                   .WithFileSystem(fileSystem)
                   .Construct())
        {
            var action1 = testMod1.Actions.First();

            var mod3 = testHelpers.CreateDummyPlugin(modKey3, mod =>
            {
                var copy = action1.DeepCopy();
                copy.EditorID = modifiedEditorId;
                mod.Actions.Add(copy);
            });
                
            mods.Add(mod3);
        }
        
        loadOrderProvider.SetTo(mods.ToArray());
        
        sut.Merge(
            modsToMerge: mods,
            outputKey: outputModKey,
            outputFolder: dataDirectory.Path);

        var outputFile = Path.Combine(dataDirectory.Path, outputModKey.FileName);
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
