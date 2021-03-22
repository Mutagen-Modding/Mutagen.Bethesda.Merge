using System.IO;
using MutagenMerger.Pex;
using Xunit;

namespace MutagenMerger.Tests
{
    public class PexTests
    {
        [Theory]
        [InlineData("Actor.pex")]
        [InlineData("Art.pex")]
        [InlineData("FormType.pex")]
        [InlineData("Game.pex")]
        [InlineData("ObjectReference.pex")]
        [InlineData("Outfit.pex")]
        [InlineData("SoulGem.pex")]
        public void TestPexParsing(string file)
        {
            var path = Path.Combine("files", file);
            Assert.True(File.Exists(path));

            var pex = PexParser.ParsePexFile(path);
        }

        [Theory]
        [InlineData("Actor.pex")]
        [InlineData("Art.pex")]
        [InlineData("FormType.pex")]
        [InlineData("Game.pex")]
        [InlineData("ObjectReference.pex")]
        [InlineData("Outfit.pex")]
        [InlineData("SoulGem.pex")]
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
