using System.IO.Abstractions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace MutagenMerger.Tests;

public class MergerTests
{
    public MergerTests()
    {
        Warmup.Init();
    }
        
    [Theory, MutagenAutoData]
    public void TestMerging(
        IFileSystem fileSystem,
        DirectoryPath existingFolder,
        MutagenTestHelpers testHelpers)
    {
        var mod1 = testHelpers.CreateDummyPlugin(existingFolder, "test-file-1.esp", mod =>
        {
            mod.Actions.AddNew("Action1");
            mod.Actions.AddNew("Action2");
        });

        var mod2 = testHelpers.CreateDummyPlugin(existingFolder, "test-file-2.esp", mod =>
        {
            mod.Actions.AddNew("Action3");
            mod.Actions.AddNew("Action4");
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

            var mod3 = testHelpers.CreateDummyPlugin(existingFolder, "test-file-3.esp", mod =>
            {
                var copy = action1.DeepCopy();
                copy.EditorID = "Action1x";
                mod.Actions.Add(copy);
            });
                
            mods.Add(mod3);
        }

        const string outputFileName = "output.esp";
        throw new NotImplementedException();
        // using (var merger = new Merger<ISkyrimModGetter, ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(testFolder, mods, mods, outputFileName, testFolder, GameRelease.SkyrimSE))
        // {
        //     merger.Merge();
        // }

        var outputFile = Path.Combine(existingFolder, outputFileName);
        testHelpers.TestPlugin(outputFile, mod =>
        {
            Assert.Equal(4, mod.Actions.Count);
                
            Assert.Contains(mod.Actions, x => x.EditorID == "Action1x");
            Assert.Contains(mod.Actions, x => x.EditorID == "Action2");
            Assert.Contains(mod.Actions, x => x.EditorID == "Action3");
            Assert.Contains(mod.Actions, x => x.EditorID == "Action4");
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
