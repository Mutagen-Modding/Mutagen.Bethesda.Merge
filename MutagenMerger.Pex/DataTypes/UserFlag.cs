using System.IO;
using JetBrains.Annotations;
using MutagenMerger.Pex.Extensions;
using MutagenMerger.Pex.Interfaces;

namespace MutagenMerger.Pex.DataTypes
{
    [PublicAPI]
    public class UserFlag : IUserFlag
    {
        public ushort NameIndex { get; set; }
        public byte FlagIndex { get; set; }

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public UserFlag() { }
        
        public UserFlag(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16BE();
            FlagIndex = br.ReadByte();
        }
    }
}
