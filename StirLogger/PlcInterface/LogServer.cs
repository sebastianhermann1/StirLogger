using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using StirLogger.RawDataStructureReader;

namespace StirLogger.PlcInterface;

public class LogServer
{
    private IPAddress _ipAddress;
    private int _port;
    private IPEndPoint _endPoint;
    private CancellationTokenSource _ctk;
    private RawDataStructure _rawDataStructure;
    private Socket _listener;

    public LogServer(BlockingCollection<LogEntry> logs, RawDataStructure rawDataStructure, string ipAdress = "0.0.0.0", int port = 5000)
    {
        _ctk = new CancellationTokenSource();
        _rawDataStructure = rawDataStructure;
        _ipAddress = IPAddress.Parse(ipAdress);
        _port = port;
        _endPoint = new IPEndPoint(_ipAddress, _port);
    }

    private async Task RunAsync()
    {
        try
        {
            // create listener socket
            _listener = new Socket(_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(_endPoint);
            _listener.Listen(1);

            while (!_ctk.Token.IsCancellationRequested)
            {
                // get logs from plc over tcp, parse the data and add to logs
                // Reconnect if connection is lost
                using Socket handler = await _listener.AcceptAsync(_ctk.Token);
                byte[] buffer = new byte[_rawDataStructure.ByteLength];
                int bytesReceived;
                while ((bytesReceived = await handler.ReceiveAsync(buffer, SocketFlags.None, _ctk.Token)) > 0)
                {
                    if (bytesReceived < _rawDataStructure.ByteLength)
                        continue;

                    if (bytesReceived == _rawDataStructure.ByteLength)
                    {
                        // Parse buffer into Usable form
                        Dictionary<string, ParsedDataObject> parsedData = RawDataParser.ParseData(_rawDataStructure, buffer);

                        // Ensure the timestamp is converted to a string before parsing
                        if (parsedData["timestamp"] is ParsedDataObject timestampObj && 
                            DateTime.TryParse(timestampObj.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp))
                        {
                            LogEntry logEntry = new LogEntry(timestamp, parsedData["message"].ToString());
                        }
                        else
                        {
                            Console.WriteLine("Invalid timestamp format.");
                        }
                        continue;
                    }

                    throw new Exception("Received more bytes than expected");
                }
                if (bytesReceived > 0)
                {
                    
                }
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void Start()
    {
        _ = Task.Run(() => RunAsync());
    }

    public void Stop()
    {
        _ctk.Cancel();
    }
}