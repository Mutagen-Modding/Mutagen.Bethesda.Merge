using System.Collections.Generic;
using System.IO;
using MutagenMerger.Pex;
using Xunit;

namespace MutagenMerger.Tests
{
    public class PexTests
    {
        public static readonly IEnumerable<object[]> TestDataFiles = new List<object[]>
        {
            //from SKSE https://skse.silverlock.org/
            new object[]{ "Actor.pex" },
            new object[]{ "Art.pex" },
            new object[]{ "FormType.pex" },
            new object[]{ "Game.pex" },
            new object[]{ "ObjectReference.pex" },
            new object[]{ "Outfit.pex" },
            new object[]{ "SoulGem.pex" },
            
            //from https://github.com/mwilsnd/SkyrimSE-SmoothCam/blob/master/CodeGen/MCM/SmoothCamMCM.pex
            new object[]{ "SmoothCamMCM.pex" },
            
            //from https://www.nexusmods.com/skyrimspecialedition/mods/18076
            new object[]{ "nwsFollowerMCMExScript.pex" },
            new object[]{ "nwsFollowerMCMScript.pex" },
    };
        
        [Theory]
        [MemberData(nameof(TestDataFiles))]
        public void TestPexParsing(string file)
        {
            var path = Path.Combine("files", file);
            Assert.True(File.Exists(path));

            var pex = PexParser.ParsePexFile(path);
        }

        [Theory]
        [MemberData(nameof(TestDataFiles))]
        public void TestPexWriting(string file)
        {
            var inputFile = Path.Combine("files", file);
            Assert.True(File.Exists(inputFile));

            var inputPex = PexParser.ParsePexFile(inputFile);

            var outputFile = Path.Combine("output", file);
            inputPex.WritePexFile(outputFile);
            Assert.True(File.Exists(outputFile));

            var outputPex = PexParser.ParsePexFile(outputFile);
            
            var inputFi = new FileInfo(inputFile);
            var outputFi = new FileInfo(outputFile);
            
            Assert.Equal(inputFi.Length, outputFi.Length);
        }
    }
}
