using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
using MutagenMerger.Lib;
using Xunit;

namespace MutagenMerger.Tests
{
    public class MergerTests
    {
        public MergerTests()
        {
            WarmupSkyrim.Init();
        }
        
        [Fact]
        public void TestMerging()
        {
            const string testFolder = "merging-test-folder";
            if (Directory.Exists(testFolder))
                Directory.Delete(testFolder, true);
            
            var mod1 = MutagenTestHelpers.CreateDummyPlugin(testFolder, "test-file-1.esp", mod =>
            {
                mod.Actions.AddNew("Action1");
                mod.Actions.AddNew("Action2");
            });

            var mod2 = MutagenTestHelpers.CreateDummyPlugin(testFolder, "test-file-2.esp", mod =>
            {
                mod.Actions.AddNew("Action3");
                mod.Actions.AddNew("Action4");
            });

            var mods = new List<string>
            {
                mod1,
                mod2,
            };

            using (var testMod1 =
                SkyrimMod.CreateFromBinaryOverlay(Path.Combine(testFolder, mod1), SkyrimRelease.SkyrimSE))
            {
                var action1 = testMod1.Actions.First();

                var mod3 = MutagenTestHelpers.CreateDummyPlugin(testFolder, "test-file-3.esp", mod =>
                {
                    var copy = action1.DeepCopy();
                    copy.EditorID = "Action1x";
                    mod.Actions.Add(copy);
                });
                
                mods.Add(mod3);
            }

            const string outputFileName = "output.esp";
            using (var merger = new Merger(testFolder, mods, outputFileName))
            {
                merger.Merge();
            }

            var outputFile = Path.Combine(testFolder, outputFileName);
            MutagenTestHelpers.TestPlugin(outputFile, mod =>
            {
                Assert.Equal(4, mod.Actions.Count);
                
                Assert.Contains(mod.Actions, x => x.EditorID == "Action1x");
                Assert.Contains(mod.Actions, x => x.EditorID == "Action2");
                Assert.Contains(mod.Actions, x => x.EditorID == "Action3");
                Assert.Contains(mod.Actions, x => x.EditorID == "Action4");
            });
        }
    }
}
