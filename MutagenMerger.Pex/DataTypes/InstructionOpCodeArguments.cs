using System.Collections.Generic;
using JetBrains.Annotations;
using MutagenMerger.Pex.Enums;

namespace MutagenMerger.Pex.DataTypes
{
    [PublicAPI]
    public static class InstructionOpCodeArguments
    {
        public static IReadOnlyList<string> Arguments => new[]
        {
            "", // 00
            "SII",
            "SFF",
            "SII",
            "SFF", // 04
            "SII",
            "SFF",
            "SII",
            "SFF", // 08
            "SII",
            "SA",
            "SI",
            "SF", // 0C
            "SA",
            "SA",
            "SAA",
            "SAA", // 10
            "SAA",
            "SAA",
            "SAA",
            "L", // 14
            "AL",
            "AL",
            "NSS*",
            "NS*", // 18
            "NNS*",
            "A",
            "SQQ",
            "NSS", // 1C
            "NSA",
            "Su",
            "SS",
            "SSI", // 20
            "SIA"
        };

        public static string GetArguments(InstructionOpcode opcode)
        {
            var index = (byte) opcode;
            return Arguments[index];
        }
    }
}
