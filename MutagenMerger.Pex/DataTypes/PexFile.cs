using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MutagenMerger.Pex.Extensions;
using MutagenMerger.Pex.Interfaces;

namespace MutagenMerger.Pex.DataTypes
{
    [PublicAPI]
    public class PexFile : IPexFile
    {
        public uint Magic { get; set; }
        
        public byte MajorVersion { get; set; }
        
        public byte MinorVersion { get; set; }
        
        public ushort GameId { get; set; }
        
        public DateTime CompilationTime { get; set; }

        public string SourceFileName { get; set; } = string.Empty;
        
        public string Username { get; set; } = string.Empty;
        
        public string MachineName { get; set; } = string.Empty;
        
        public IStringTable? StringTable { get; set; }
        
        public IDebugInfo? DebugInfo { get; set; }

        public List<IUserFlag> UserFlags { get; set; } = new();

        public List<IPexObject> Objects { get; set; } = new();

        public PexFile() { }
        public PexFile(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            Magic = br.ReadUInt32BE();
            if (Magic != 0xFA57C0DE)
                throw new NotImplementedException("File Header is not FASTCODE! Who the hell thought this was a nice magic number...");
            
            MajorVersion = br.ReadByte();
            MinorVersion = br.ReadByte();
            GameId = br.ReadUInt16BE();
            CompilationTime = br.ReadUInt64BE().ToDateTime();
            SourceFileName = br.ReadWString();
            Username = br.ReadWString();
            MachineName = br.ReadWString();

            StringTable = new StringTable(br);

            DebugInfo = new DebugInfo(br);

            var userFlagCount = br.ReadUInt16BE();
            for (var i = 0; i < userFlagCount; i++)
            {
                var userFlag = new UserFlag(br);
                UserFlags.Add(userFlag);
            }

            var objectCount = br.ReadUInt16BE();
            for (var i = 0; i < objectCount; i++)
            {
                var pexObject = new PexObject(br);
                Objects.Add(pexObject);
            }
        }
    }
}
