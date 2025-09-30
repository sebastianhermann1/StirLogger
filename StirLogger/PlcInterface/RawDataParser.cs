using System;
using System.Collections.Generic;
using StirLogger.RawDataStructureReader;
using StirLogger.RawDataStructureReader.Models;

namespace StirLogger.PlcInterface;


public static class RawDataParser
{
    public static Dictionary<string, ParsedDataObject> ParseData(RawDataStructure structure, byte[] rawData)
    {
        Dictionary<string, ParsedDataObject> result = new Dictionary<string, ParsedDataObject>();
        foreach (KeyValuePair<string, FieldInfo> kvp in structure.Fields)
        {
            string name = kvp.Key;
            FieldInfo info = kvp.Value;
            object? value = null;

            switch (info.Type)
            {
                case DataPointType.BOOL:
                    // Lies das Bit an ByteOffset und BitOffset
                    int boolByte = rawData[info.ByteOffset];
                    value = ((boolByte >> info.BitOffset) & 0x01) == 1;
                    break;
                case DataPointType.DINT:
                    value = ParserUtils.ReadInt32BE(rawData, info.ByteOffset);
                    break;
                case DataPointType.UDINT:
                    value = ParserUtils.ReadUInt32BE(rawData, info.ByteOffset);
                    break;
                case DataPointType.REAL:
                    value = ParserUtils.ReadFloatBE(rawData, info.ByteOffset);
                    break;
                case DataPointType.LREAL:
                    value = ParserUtils.ReadDoubleBE(rawData, info.ByteOffset);
                    break;
                case DataPointType.STRING:
                    value = ParserUtils.ReadString(rawData, info.ByteOffset + 2,
                        info.Length - 2); // TODO: fix workaround
                    break;
                case DataPointType.DTL:
                    value = ParserUtils.ReadDTL(rawData, info.ByteOffset);
                    break;
                default:
                    value = null;
                    break;
            }
            if(value == null) throw new Exception($"Failed to parse metadata field {name} of type {info.Type}");
            
            result[name] = new ParsedDataObject(value, info.Type);
        }
        
        return result;
    }
}