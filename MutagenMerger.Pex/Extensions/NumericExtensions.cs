using System;

namespace MutagenMerger.Pex.Extensions
{
    internal static class NumericExtensions
    {
        internal static DateTime ToDateTime(this ulong value)
        {
            return DateTime.UnixEpoch.AddSeconds(value);
        }
    }
}
