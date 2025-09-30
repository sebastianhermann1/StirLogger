using System.Collections.Generic;
using StirLogger.ConfigReader;

namespace StirLogger.Server;

public struct ValueStruct
{   
    public string type;
    public object value;
}

public static class Parser
{
    public static Dictionary<string, ValueStruct> ParseData(RawDataStructure structure, byte[] rawData)
    {
        Dictionary<string, ValueStruct> data = new Dictionary<string, ValueStruct>();
        foreach (KeyValuePair<string, FieldInfo> kvp in structure.Fields)
        {
            
        }
        
        return data;
    }
}