using System;
using System.IO;
using System.Text.RegularExpressions;

namespace StirLogger.ConfigReader;

/// <summary>
/// Liest die Header-Beschreibung aus einer Datei und erzeugt eine HeaderStruct-Instanz.
/// </summary>
public static class DataStructureReader
{
    /// <summary>
    /// Liest die Header-Beschreibung und gibt eine Instanz von HeaderStruct zurück.
    /// </summary>
    public static RawDataStructure Read(string metadataFilePath)
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