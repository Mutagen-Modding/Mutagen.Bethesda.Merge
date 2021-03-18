using JetBrains.Annotations;

namespace MutagenMerger.Pex.Interfaces
{
    [PublicAPI]
    public interface IUserFlag : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public byte FlagIndex { get; set; }

        public string GetName(IStringTable stringTable);
    }
}
