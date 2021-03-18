using System.Buffers.Binary;
using System.IO;

namespace MutagenMerger.Pex.Extensions
{
    internal static class BinaryReaderExtensions
    {
        internal static string ReadWString(this BinaryReader br, bool bigEndian = true)
        {
            var length = bigEndian ? br.ReadUInt16BE() : br.ReadUInt16();
            if (length == 0) return string.Empty;
            var chars = br.ReadChars(length);
            return new string(chars);
        }

        internal static ushort ReadUInt16BE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(ushort));
            return BinaryPrimitives.ReadUInt16BigEndian(bytes);
        }

        internal static uint ReadUInt32BE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(uint));
            return BinaryPrimitives.ReadUInt32BigEndian(bytes);
        }

        internal static ulong ReadUInt64BE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(ulong));
            return BinaryPrimitives.ReadUInt64BigEndian(bytes);
        }

        internal static int ReadInt32BE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(bytes);
        }

        internal static float ReadSingleBE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(bytes);
        }
    }
}
