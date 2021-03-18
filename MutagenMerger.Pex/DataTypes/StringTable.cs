using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MutagenMerger.Pex.Extensions;
using MutagenMerger.Pex.Interfaces;

namespace MutagenMerger.Pex.DataTypes
{
    [PublicAPI]
    public class StringTable : IStringTable
    {
        public List<string> Strings { get; set; } = new();
     
        public StringTable() { }
        
        public StringTable(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            var count = br.ReadUInt16BE();
            for (var i = 0; i < count; i++)
            {
                Strings.Add(br.ReadWString());
            }
        }

        public string GetFromIndex(ushort index)
        {
            return Strings[index];
        }
    }
}
