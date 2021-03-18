using System.Collections.Generic;
using JetBrains.Annotations;
using MutagenMerger.Pex.Enums;

namespace MutagenMerger.Pex.Interfaces
{
    [PublicAPI]
    public interface IPexObject : IBinaryObject
    {
        public ushort NameIndex { get; set; }

        //TODO: merge IPexObject with IPexObjectData, no point in having 2 classes for this
        
        public IPexObjectData? Data { get; set; }
        
        public string GetName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectData : IBinaryObject
    {
        public ushort ParentClassNameIndex { get; set; }
        
        public ushort DocStringIndex { get; set; }
        
        public uint UserFlags { get; set; }
        
        public ushort AutoStateNameIndex { get; set; }
        
        public List<IPexObjectDataVariable> Variables { get; set; }
        
        public List<IPexObjectDataProperty> Properties { get; set; }
        
        public List<IPexObjectDataState> States { get; set; }

        public string GetParentClassName(IStringTable stringTable);
        public string GetDocString(IStringTable stringTable);
        public string GetAutoStateName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectDataVariable : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public ushort TypeNameIndex { get; set; }
        
        public uint UserFlags { get; set; }
        
        public IPexObjectDataVariableData? VariableData { get; set; }

        public string GetName(IStringTable stringTable);
        public string GetTypeName(IStringTable stringTable);
    }
    
    [PublicAPI]
    public interface IPexObjectDataVariableData : IBinaryObject
    {
        public VariableType VariableType { get; set; }
        
        public ushort? StringValueIndex { get; set; }
        
        public int? IntValue { get; set; }
        
        public float? FloatValue { get; set; }
        
        public bool? BoolValue { get; set; }

        public string? GetStringValue(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectDataProperty : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public ushort TypeNameIndex { get; set; }
        
        public ushort DocStringIndex { get; set; }
        
        public uint UserFlags { get; set; }
        
        public PropertyFlags Flags { get; set; }
        
        public ushort? AutoVarNameIndex { get; set; }
        
        public IPexObjectFunction? ReadHandler { get; set; }
        
        public IPexObjectFunction? WriteHandler { get; set; }

        public string GetName(IStringTable stringTable);
        public string GetTypeName(IStringTable stringTable);
        public string? GetAutoVarName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectDataState : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public List<IPexObjectNamedFunction> Functions { get; set; }

        public string GetName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectNamedFunction : IBinaryObject
    {
        public ushort FunctionNameIndex { get; set; }

        public IPexObjectFunction? Function { get; set; }
        
        public string GetFunctionName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectFunction : IBinaryObject
    {
        public ushort ReturnTypeNameIndex { get; set; }
        
        public ushort DocStringIndex { get; set; }
        
        public uint UserFlags { get; set; }
        
        public FunctionFlags Flags { get; set; }
        
        public List<IPexObjectFunctionVariable> Parameters { get; set; }
        
        public List<IPexObjectFunctionVariable> Locals { get; set; }
        
        public List<IPexObjectFunctionInstruction> Instructions { get; set; }

        public string GetReturnTypeName(IStringTable stringTable);
        public string GetDocString(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectFunctionVariable : IBinaryObject
    {
        public ushort NameIndex { get; set; }
        
        public ushort TypeNameIndex { get; set; }

        public string GetName(IStringTable stringTable);
        public string GetTypeName(IStringTable stringTable);
    }

    [PublicAPI]
    public interface IPexObjectFunctionInstruction : IBinaryObject
    {
        public InstructionOpcode OpCode { get; set; }
        
        public List<IPexObjectDataVariableData> Arguments { get; set; }
    }
}
