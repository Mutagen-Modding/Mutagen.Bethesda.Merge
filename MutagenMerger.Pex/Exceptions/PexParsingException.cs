using System;
using JetBrains.Annotations;

namespace MutagenMerger.Pex.Exceptions
{
    [PublicAPI]
    public class PexParsingException : Exception
    {
        public PexParsingException(string msg) : base(msg) { }
        public PexParsingException(string msg, Exception e) : base(msg, e) { }
    }
}
