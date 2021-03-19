using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using MutagenMerger.Pex.Enums;
using MutagenMerger.Pex.Exceptions;
using MutagenMerger.Pex.Extensions;
using MutagenMerger.Pex.Interfaces;

namespace MutagenMerger.Pex.DataTypes
{
    [PublicAPI]
    public class PexObject : IPexObject
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;

        public ushort ParentClassNameIndex { get; set; } = ushort.MaxValue;
        public ushort DocStringIndex { get; set; } = ushort.MaxValue;
        public uint UserFlags { get; set; } = uint.MaxValue;
        public ushort AutoStateNameIndex { get; set; } = ushort.MaxValue;
        public List<IPexObjectVariable> Variables { get; set; } = new();
        public List<IPexObjectProperty> Properties { get; set; } = new();
        public List<IPexObjectState> States { get; set; } = new();
        
        public string GetParentClassName(IStringTable stringTable) => stringTable.GetFromIndex(ParentClassNameIndex);

        public string GetDocString(IStringTable stringTable) => stringTable.GetFromIndex(DocStringIndex);

        public string GetAutoStateName(IStringTable stringTable) => stringTable.GetFromIndex(AutoStateNameIndex);

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);
        
        public PexObject() { }
        public PexObject(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16BE();
            
            /*
             * This is the size of the entire object in bytes not some count variable for a loop. This also includes
             * the size of itself thus the - sizeof(uint)
             */
            var size = br.ReadUInt32BE() - sizeof(uint);
            var currentPos = br.BaseStream.Position;
            
            ParentClassNameIndex = br.ReadUInt16BE();
            DocStringIndex = br.ReadUInt16BE();
            UserFlags = br.ReadUInt32BE();
            AutoStateNameIndex = br.ReadUInt16BE();

            var variables = br.ReadUInt16BE();
            for (var i = 0; i < variables; i++)
            {
                var variable = new PexObjectVariable(br);
                Variables.Add(variable);
            }

            var properties = br.ReadUInt16BE();
            for (var i = 0; i < properties; i++)
            {
                var property = new PexObjectProperty(br);
                Properties.Add(property);
            }

            var states = br.ReadUInt16BE();
            for (var i = 0; i < states; i++)
            {
                var state = new PexObjectState(br);
                States.Add(state);
            }

            var newPos = br.BaseStream.Position;
            if (newPos != currentPos + size)
                throw new PexParsingException("Current position in Stream does not match expected position: " +
                                              $"Current: {newPos} Expected: {currentPos + size}");
        }
    }

    [PublicAPI]
    public class PexObjectVariable : IPexObjectVariable
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public ushort TypeNameIndex { get; set; } = ushort.MaxValue;
        public uint UserFlags { get; set; } = uint.MaxValue;
        public IPexObjectVariableData? VariableData { get; set; }
        
        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public string GetTypeName(IStringTable stringTable) => stringTable.GetFromIndex(TypeNameIndex);
        
        public PexObjectVariable() { }
        public PexObjectVariable(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16BE();
            TypeNameIndex = br.ReadUInt16BE();
            UserFlags = br.ReadUInt32BE();

            VariableData = new PexObjectVariableData(br);
        }
    }

    [PublicAPI]
    public class PexObjectVariableData : IPexObjectVariableData
    {
        public VariableType VariableType { get; set; } = VariableType.Null;
        public ushort? StringValueIndex { get; set; }
        public int? IntValue { get; set; }
        public float? FloatValue { get; set; }
        public bool? BoolValue { get; set; }

        public string? GetStringValue(IStringTable stringTable) =>
            StringValueIndex.HasValue ? stringTable.GetFromIndex(StringValueIndex.Value) : null; 
        
        public PexObjectVariableData() { }
        public PexObjectVariableData(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            VariableType = (VariableType) br.ReadByte();
            switch (VariableType)
            {
                case VariableType.Null:
                    break;
                case VariableType.Identifier:
                case VariableType.String:
                    StringValueIndex = br.ReadUInt16BE();
                    break;
                case VariableType.Integer:
                    IntValue = br.ReadInt32BE();
                    break;
                case VariableType.Float:
                    FloatValue = br.ReadSingleBE();
                    break;
                case VariableType.Bool:
                    //TODO: use ReadByte instead?
                    BoolValue = br.ReadBoolean();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [PublicAPI]
    public class PexObjectProperty : IPexObjectProperty
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public ushort TypeNameIndex { get; set; } = ushort.MaxValue;
        public ushort DocStringIndex { get; set; } = ushort.MaxValue;
        public uint UserFlags { get; set; } = uint.MaxValue;
        public PropertyFlags Flags { get; set; }
        public ushort? AutoVarNameIndex { get; set; }
        public IPexObjectFunction? ReadHandler { get; set; }
        public IPexObjectFunction? WriteHandler { get; set; }

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public string GetTypeName(IStringTable stringTable) => stringTable.GetFromIndex(TypeNameIndex);

        public string? GetAutoVarName(IStringTable stringTable) =>
            AutoVarNameIndex.HasValue ? stringTable.GetFromIndex(AutoVarNameIndex.Value) : null;
        
        public PexObjectProperty() { }
        public PexObjectProperty(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16BE();
            TypeNameIndex = br.ReadUInt16BE();
            DocStringIndex = br.ReadUInt16BE();
            UserFlags = br.ReadUInt32BE();

            var flags = br.ReadByte();
            Flags = (PropertyFlags) flags;
            
            if ((flags & 4) != 0)
            {
                AutoVarNameIndex = br.ReadUInt16BE();
            }

            if ((flags & 5) == 1)
            {
                ReadHandler = new PexObjectFunction(br);
            }

            if ((flags & 6) == 2)
            {
                WriteHandler = new PexObjectFunction(br);
            }
        }
    }

    [PublicAPI]
    public class PexObjectState : IPexObjectState
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;

        public List<IPexObjectNamedFunction> Functions { get; set; } = new();

        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);
        
        public PexObjectState() { }
        public PexObjectState(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16BE();

            var functions = br.ReadUInt16BE();
            for (var i = 0; i < functions; i++)
            {
                var namedFunction = new PexObjectNamedFunction(br);
                Functions.Add(namedFunction);
            }
        }
    }

    [PublicAPI]
    public class PexObjectNamedFunction : IPexObjectNamedFunction
    {
        public ushort FunctionNameIndex { get; set; } = ushort.MaxValue;
        
        public IPexObjectFunction? Function { get; set; }

        public string GetFunctionName(IStringTable stringTable) => stringTable.GetFromIndex(FunctionNameIndex);
        
        public PexObjectNamedFunction() { }
        public PexObjectNamedFunction(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            FunctionNameIndex = br.ReadUInt16BE();
            Function = new PexObjectFunction(br);
        }
    }

    [PublicAPI]
    public class PexObjectFunction : IPexObjectFunction
    {
        public ushort ReturnTypeNameIndex { get; set; } = ushort.MaxValue;
        public ushort DocStringIndex { get; set; } = ushort.MaxValue;
        public uint UserFlags { get; set; } = uint.MaxValue;
        public FunctionFlags Flags { get; set; }
        public List<IPexObjectFunctionVariable> Parameters { get; set; } = new();
        public List<IPexObjectFunctionVariable> Locals { get; set; } = new();
        public List<IPexObjectFunctionInstruction> Instructions { get; set; } = new();
        
        public string GetReturnTypeName(IStringTable stringTable) => stringTable.GetFromIndex(ReturnTypeNameIndex);
        public string GetDocString(IStringTable stringTable) => stringTable.GetFromIndex(DocStringIndex);
        
        public PexObjectFunction() { }
        public PexObjectFunction(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            ReturnTypeNameIndex = br.ReadUInt16BE();
            DocStringIndex = br.ReadUInt16BE();
            UserFlags = br.ReadUInt32BE();
            Flags = (FunctionFlags) br.ReadByte();

            var parameters = br.ReadUInt16BE();
            for (var i = 0; i < parameters; i++)
            {
                var parameter = new PexObjectFunctionVariable(br);
                Parameters.Add(parameter);
            }
            
            var locals = br.ReadUInt16BE();
            for (var i = 0; i < locals; i++)
            {
                var local = new PexObjectFunctionVariable(br);
                Locals.Add(local);
            }
            
            var instructions = br.ReadUInt16BE();
            for (var i = 0; i < instructions; i++)
            {
                var instruction = new PexObjectFunctionInstruction(br);
                Instructions.Add(instruction);
            }
        }
    }

    [PublicAPI]
    public class PexObjectFunctionVariable : IPexObjectFunctionVariable
    {
        public ushort NameIndex { get; set; } = ushort.MaxValue;
        public ushort TypeNameIndex { get; set; } = ushort.MaxValue;
        
        public string GetName(IStringTable stringTable) => stringTable.GetFromIndex(NameIndex);

        public string GetTypeName(IStringTable stringTable) => stringTable.GetFromIndex(TypeNameIndex);
        
        public PexObjectFunctionVariable() { }
        public PexObjectFunctionVariable(BinaryReader br) { Read(br); }
        
        public void Read(BinaryReader br)
        {
            NameIndex = br.ReadUInt16BE();
            TypeNameIndex = br.ReadUInt16BE();
        }
    }

    [PublicAPI]
    public class PexObjectFunctionInstruction : IPexObjectFunctionInstruction
    {
        public InstructionOpcode OpCode { get; set; } = InstructionOpcode.nop;
        public List<IPexObjectVariableData> Arguments { get; set; } = new();
        
        public PexObjectFunctionInstruction() { }
        public PexObjectFunctionInstruction(BinaryReader br) { Read(br); }

        public void Read(BinaryReader br)
        {
            OpCode = (InstructionOpcode) br.ReadByte();
            
            var arguments = InstructionOpCodeArguments.GetArguments(OpCode);
            foreach (var current in arguments)
            {
                var argument = new PexObjectVariableData(br);
                Arguments.Add(argument);

                if (current == '*')
                {
                    if (argument.VariableType != VariableType.Integer || !argument.IntValue.HasValue)
                        throw new PexParsingException($"Variable-Length Arguments require an Integer Argument! Argument is {argument.VariableType}");
                    for (var i = 0; i < argument.IntValue.Value; i++)
                    {
                        var anotherArgument = new PexObjectVariableData(br);
                        Arguments.Add(anotherArgument);
                    }
                }

                if (current == 'u')
                {
                    //u: unsigned integer?
                    Debugger.Break();
                    throw new NotImplementedException("Argument Type 'u' reached. Please report this on GitHub and attach the pex file.");
                }
            }
        }
    }
}
