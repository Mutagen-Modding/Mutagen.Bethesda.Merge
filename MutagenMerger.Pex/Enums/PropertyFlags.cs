using System;
using JetBrains.Annotations;

namespace MutagenMerger.Pex.Enums
{
    [PublicAPI]
    [Flags]
    public enum PropertyFlags : byte
    {
        Read = 1,
        Write = 2,
        AutoVar = 4
    }
}
