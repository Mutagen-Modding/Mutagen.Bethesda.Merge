using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MutagenMerger.Pex.Interfaces
{
    [PublicAPI]
    public interface IDebugInfo : IBinaryObject
    {
        public bool HasDebugInfo { get; set; }
        
        public DateTime ModificationTime { get; set; }
        
        public List<IDebugFunction> Functions { get; set; }
    }

    [PublicAPI]
    public interface IDebugFunction : IBinaryObject
    {
        public ushort ObjectNameIndex { get; set; }
        
        public ushort StateNameIndex { get; set; }
        
        public ushort FunctionNameIndex { get; set; }
        
        //TODO: make enum
        public byte FunctionType { get; set; }
        
        public ushort InstructionCount { get; set; }
        
        public List<ushort> LineNumbers { get; set; }

        public string GetObjectName(IStringTable stringTable);
        public string GetStateName(IStringTable stringTable);
        public string GetFunctionName(IStringTable stringTable);
    }
}
