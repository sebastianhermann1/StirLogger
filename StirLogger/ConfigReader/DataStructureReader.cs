using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace StirLogger.ConfigReader;

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

/// <summary>
/// Liest die Header-Beschreibung aus einer Datei und erzeugt eine HeaderStruct-Instanz.
/// </summary>
public class DataStructureReader
{
    private readonly string metadataFilePath;

    /// <summary>
    /// Erstellt instanz zum Auslesen des Exports von TIA-Portal
    /// </summary>
    /// <param name="metadataFilePath">Pfad zum Metadata export</param>
    public DataStructureReader(string metadataFilePath)
    {
        this.metadataFilePath = metadataFilePath;
    }


    /// <summary>
    /// Liest die Header-Beschreibung und gibt eine Instanz von HeaderStruct zurück.
    /// </summary>
    public RawDataStructure Read()
    {
        string[] lines = File.ReadAllLines(metadataFilePath);
        var metadata = new RawDataStructure();

        bool inStruct = false;
        Regex fieldRegex = new Regex(@"(\w+)\s*(?:\{[^\}]*\})?\s*:\s*(\w+);");
        int byteoffset = 0;
        int bitoffset = 0;

        string? lastFieldName = null;
        string? lastFieldType = null;

        foreach (var line in lines)
        {
            if (line.Trim().StartsWith("STRUCT"))
            {
                inStruct = true;
                continue;
            }

            if (line.Trim().StartsWith("END_STRUCT"))
            {
                break;
            }

            if (!inStruct) continue;

            var match = fieldRegex.Match(line);
            if (match.Success)
            {
                string fieldName = match.Groups[1].Value.Trim();
                string fieldType = match.Groups[2].Value.Trim();

                int length = ConfigReaderUtils.GetTypeLength(fieldType);

                if (length > 0)
                {
                    if (bitoffset != 0)
                    {
                        if (byteoffset % 2 == 1)
                        {
                            byteoffset += 1;
                        }
                        else
                        {
                            byteoffset += 2;
                        }

                        bitoffset = 0;
                    }
                }

                metadata.Fields[fieldName] = new FieldInfo
                {
                    Type = fieldType,
                    ByteOffset = byteoffset,
                    BitOffset = bitoffset,
                    Length = length
                };

                lastFieldName = fieldName;
                lastFieldType = fieldType;

                if (length == 0)
                {
                    if (bitoffset == 7)
                    {
                        bitoffset = 0;
                        byteoffset++;
                    }
                    else
                    {
                        bitoffset++;
                    }
                }

                byteoffset += length;
            }
        }

        if (lastFieldType != null && string.Equals(lastFieldType, "Bool", StringComparison.OrdinalIgnoreCase))
        {
            if (byteoffset % 2 != 0)
            {
                metadata.ByteLength = byteoffset + 1;
            }
            else
            {
                metadata.ByteLength = byteoffset + 2;
            }
        }
        else
        {
            metadata.ByteLength = byteoffset;
        }

        return metadata;
    }
}