using System;
using StirLogger.RawDataStructureReader.Models;

namespace StirLogger.RawDataStructureReader;

public class ConfigReaderUtils
{
    /// <summary>
    /// Gibt die Byte-Länge des jeweiligen Typs zurück.
    /// </summary>
    public static int GetTypeLength(string type)
    {
        switch (type.ToUpper())
        {
            case DataPointType.BOOL: return 0;
            case DataPointType.INT: return 2;
            case DataPointType.DINT:
            case DataPointType.UDINT:
            case DataPointType.REAL: return 4;
            case DataPointType.LREAL: return 8;
            case DataPointType.DTL: return 12;
            case DataPointType.STRING: return 256;
            default: throw new Exception("Datatype unknown");
        }
    }
}