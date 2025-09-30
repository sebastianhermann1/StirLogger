using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StirLogger.ConfigReader.Models;

public class DataPointTypeConverter : JsonConverter<DataPointType>
{
    public override DataPointType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (str == null) throw new JsonException("Null value not allowed for DataPointType");
        return str; // implicit operator will validate
    }

    public override void Write(Utf8JsonWriter writer, DataPointType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

[JsonConverter(typeof(DataPointTypeConverter))]
public class DataPointType
{
    private readonly string _value;
    public const string BOOL = "BOOL";
    public const string INT = "INT";
    public const string DINT = "DINT";
    public const string UDINT = "UDINT";
    public const string REAL = "REAL";
    public const string LREAL = "LREAL";
    public const string STRING = "STRING";
    public const string DTL = "DTL";
        
    public static readonly ObservableCollection<string> Values = [BOOL, INT, DINT, UDINT, REAL, STRING, DTL, LREAL];

    private DataPointType(string value)
    {
        if (!Validate(value))
        {
            throw new ArgumentException("Invalid DataPointType value");
        }
        _value = value.ToUpper();
    }
        
    public static bool operator ==(DataPointType a, DataPointType b) => a._value == b._value;
    public static bool operator !=(DataPointType a, DataPointType b) => a._value != b._value;
    public override bool Equals(object? obj) => obj is DataPointType d && d._value == _value;
    public override int GetHashCode() => _value.GetHashCode();
    public static bool Validate(string s) => Values.Contains(s.ToUpper());
    public static implicit operator string(DataPointType d) => d._value;
    public static implicit operator DataPointType(string s) => new(s);
    public override string ToString() => _value;
}