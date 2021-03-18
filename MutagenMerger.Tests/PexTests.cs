using System.IO;
using MutagenMerger.Pex;
using Xunit;

namespace MutagenMerger.Tests
{
    public class PexTests
    {
        [Theory]
        [InlineData("Art.pex")]
        [InlineData("Outfit.pex")]
        [InlineData("SoulGem.pex")]
        public void TestPexParsing(string file)
        {
            var path = Path.Combine("files", file);
            Assert.True(File.Exists(path));

            var pex = PexParser.ParsePexFile(path);
        }
    }
}
