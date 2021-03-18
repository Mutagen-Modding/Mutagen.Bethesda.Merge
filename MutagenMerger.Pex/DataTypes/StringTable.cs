using System;
using System.IO;
using JetBrains.Annotations;
using MutagenMerger.Pex.Extensions;
using MutagenMerger.Pex.Interfaces;

namespace MutagenMerger.Pex.DataTypes
{
    [PublicAPI]
    public class StringTable : IStringTable
    {
        public string[] Strings { get; set; } = Array.Empty<string>();
     
        public StringTable() { }
        
        public StringTable(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            var count = br.ReadUInt16BE();
            Strings = new string[count];
            
            for (var i = 0; i < count; i++)
            {
                Strings[i] = br.ReadWString();
            }
        }

        public string GetFromIndex(ushort index)
        {
            return Strings[index];
        }
    }
}
