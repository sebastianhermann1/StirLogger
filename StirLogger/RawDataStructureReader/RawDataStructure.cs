using System.Collections.Generic;
using System.Linq;

namespace StirLogger.RawDataStructureReader;

/// <summary>
/// Repräsentiert die Struktur der Metadaten als Dictionary.
/// Enthält auch die Gesamtlänge der Struktur
/// </summary>
public class RawDataStructure
{
    public Dictionary<string, FieldInfo> Fields { get; set; } = new();

    /// <summary>
    /// Gesamtlänge der Struktur in Bytes.
    /// </summary>
    public int ByteLength { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not RawDataStructure other)
            return false;

        if (ByteLength != other.ByteLength)
            return false;

        if (Fields.Count != other.Fields.Count)
            return false;

        foreach (var kvp in Fields)
        {
            if (!other.Fields.TryGetValue(kvp.Key, out var otherField))
                return false;
            if (!Equals(kvp.Value, otherField))
                return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = ByteLength;
        foreach (var kvp in Fields)
        {
            hash = hash * 31 + kvp.Key.GetHashCode();
            hash = hash * 31 + (kvp.Value?.GetHashCode() ?? 0);
        }

        return hash;
    }

    public override string ToString()
    {
        var fields = string.Join(", ", Fields.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        return $"Fields: {{ {fields} }}, Length: {ByteLength}";
    }
}