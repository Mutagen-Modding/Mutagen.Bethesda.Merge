﻿using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace MutagenMerger.Pex.Extensions
{
    internal static class BinaryWriterExtensions
    {
        internal static void WriteWString(this BinaryWriter bw, string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            bw.WriteUInt16BE((ushort) s.Length);
            bw.Write(bytes);
        }

        internal static void WriteUInt16BE(this BinaryWriter bw, ushort value)
        {
            Span<byte> bytes = stackalloc byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(bytes, value);
            bw.Write(bytes);
        }
        
        internal static void WriteUInt32BE(this BinaryWriter bw, uint value)
        {
            Span<byte> bytes = stackalloc byte[sizeof(uint)];
            BinaryPrimitives.WriteUInt32BigEndian(bytes, value);
            bw.Write(bytes);
        }
        
        internal static void WriteUInt64BE(this BinaryWriter bw, ulong value)
        {
            Span<byte> bytes = stackalloc byte[sizeof(ulong)];
            BinaryPrimitives.WriteUInt64BigEndian(bytes, value);
            bw.Write(bytes);
        }
        
        internal static void WriteInt32BE(this BinaryWriter bw, int value)
        {
            Span<byte> bytes = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(bytes, value);
            bw.Write(bytes);
        }
        
        internal static void WriteSingleBE(this BinaryWriter bw, float value)
        {
            Span<byte> bytes = stackalloc byte[sizeof(float)];
            BinaryPrimitives.WriteSingleBigEndian(bytes, value);
            bw.Write(bytes);
        }
    }
}
