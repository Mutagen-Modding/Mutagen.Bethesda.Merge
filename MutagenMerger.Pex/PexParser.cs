using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using MutagenMerger.Pex.DataTypes;
using MutagenMerger.Pex.Interfaces;

namespace MutagenMerger.Pex
{
    public static class PexParser
    {
        public static IPexFile ParsePexFile(string file)
        {
            if (!File.Exists(file))
                throw new ArgumentException($"Input file does not exist {file}!", nameof(file));
            
            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var br = new BinaryReader(fs, Encoding.UTF8);

            //https://en.uesp.net/wiki/Skyrim_Mod:Compiled_Script_File_Format
            var pexFile = new PexFile(br);

            if (fs.Position != fs.Length)
                throw new NotImplementedException("Finished reading but we are not at the end of the stream!");
            
            return pexFile;
        }
    }
}
