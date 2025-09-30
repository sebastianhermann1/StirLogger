using System;
using StirLogger.RawDataStructureReader.Models;

namespace StirLogger.RawDataStructureReader;

public class ParsedDataObject(object value, DataPointType type)
{
    public readonly object Value = value;
    public readonly DataPointType Type = type;

    public string GetStringValue(string format = "o")
    {
        return Type.ToString() switch
        {
            DataPointType.STRING => (string)Value,
            DataPointType.BOOL => Convert.ToString((bool)Value),
            DataPointType.UDINT => Convert.ToString((uint)Value),
            DataPointType.DINT => Convert.ToString((int)Value),
            DataPointType.INT => Convert.ToString((short)Value),
            DataPointType.REAL => Convert.ToString((float)Value),
            DataPointType.LREAL => Convert.ToString((double)Value),
            DataPointType.DTL => ((DateTime)Value).ToString(format),
            _ => throw new Exception("unsupported data type")
        };
    }

    public static ParsedDataObject? FromString(string str, string type)
    {
        if (!DataPointType.Validate(type)) return null;
        try
        {
            object value = type switch
            {
                DataPointType.BOOL => Convert.ToBoolean(str),
                DataPointType.DINT => Convert.ToInt32(str),
                DataPointType.INT => Convert.ToInt16(str),
                DataPointType.UDINT => Convert.ToUInt32(str),
                DataPointType.REAL => Convert.ToSingle(str),
                DataPointType.LREAL => Convert.ToDouble(str),
                DataPointType.DTL => DateTime.ParseExact(str, "O", null),
                DataPointType.STRING => str,
                _ => throw new Exception($"Invalid data point type")
            };
            return new ParsedDataObject(value, type);
        } catch
        {
            return null;
        }
    }
}