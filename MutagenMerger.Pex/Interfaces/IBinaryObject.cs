using System.IO;
using JetBrains.Annotations;

namespace MutagenMerger.Pex.Interfaces
{
    [PublicAPI]
    public interface IBinaryObject
    {
        public void Read(BinaryReader br);
    }
}
