using StirLogger.RawDataStructureReader.Models;

namespace StirLogger.RawDataStructureReader;

/// <summary>
/// Enthält Informationen zu einem Header-Feld (Typ, Offset usw.).
/// </summary>
public class FieldInfo
{
    public required DataPointType Type { get; init; }
    public required int ByteOffset { get; init; }
    public required int BitOffset { get; init; }
    public required int Length { get; init; }
    
    public override string ToString()
    {
        return $"Type: {Type}, BitOffset: {BitOffset}, ByteOffset: {ByteOffset}, Length: {Length}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not FieldInfo other)
            return false;

        return Type == other.Type
               && ByteOffset == other.ByteOffset
               && BitOffset == other.BitOffset
               && Length == other.Length;
    }

    public override int GetHashCode()
    {
        int hash = Type?.GetHashCode() ?? 0;
        hash = hash * 31 + ByteOffset;
        hash = hash * 31 + BitOffset;
        hash = hash * 31 + Length;
        return hash;
    }
}