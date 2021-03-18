using JetBrains.Annotations;

namespace MutagenMerger.Pex.Interfaces
{
    [PublicAPI]
    public interface IStringTable : IBinaryObject
    {
        public string GetFromIndex(ushort index);
    }
}
