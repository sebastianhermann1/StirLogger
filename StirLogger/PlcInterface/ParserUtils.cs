using System;
using System.Text;

namespace StirLogger.PlcInterface;

public static class ParserUtils
{
    
    public static int ReadInt32BE(byte[] data, int offset)
    {
        return (data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3];
    }

    public static uint ReadUInt32BE(byte[] data, int offset)
    {
        return (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
    }

    public static float ReadFloatBE(byte[] data, int offset)
    {
        var bytes = new byte[4];
        Array.Copy(data, offset, bytes, 0, 4);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }
    
    public static double ReadDoubleBE(byte[] data, int offset)
    {
        var bytes = new byte[8];
        Array.Copy(data, offset, bytes, 0, 8);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }


    public static string ReadString(byte[] data, int offset, int length)
    {
        int realLen = Array.IndexOf<byte>(data, 0, offset, length);
        if (realLen == -1) realLen = length;
        else realLen = realLen - offset;
        return Encoding.ASCII.GetString(data, offset, realLen);
    }

    public static DateTime ReadDTL(byte[] data, int offset)
    {
        // Annahme: DTL ist 12 Byte (Jahr, Monat, Tag, Stunde, Minute, Sekunde, ms, Wochentag, ...), je nach SPS
        // Beispiel für Siemens DTL: [0]=Jahr_hi, [1]=Jahr_lo, [2]=Monat, [3]=Tag, [5]=Stunde, [6]=Minute, [7]=Sekunde, [8]=ms_hi, [9]=ms_lo
        int year = (data[offset] << 8) | data[offset + 1];
        int month = data[offset + 2];
        int day = data[offset + 3];
        int hour = data[offset + 5];
        int minute = data[offset + 6];
        int second = data[offset + 7];
        int ns = (data[offset + 8] << 8) | data[offset + 9];
        int ms = ns / 1000;
        try
        {
            return new DateTime(year, month, day, hour, minute, second,  ms);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}