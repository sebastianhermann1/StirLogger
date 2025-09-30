using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using StirLogger.PlcInterface;
using StirLogger.RawDataStructureReader;


namespace StirLogger;

class Program
{
    static int Main(string[] args)
    {
        RawDataStructure rawDataStructure = DataStructureReader.Read(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StirLogger",
                "datastructure"));

        BlockingCollection<LogEntry> logs = new BlockingCollection<LogEntry>();
        LogServer logServer = new LogServer(logs, rawDataStructure);
        SQLiteParser parser = new SQLiteParser(logs);

        // Start tasks
        logServer.Start();
        parser.Start();

        while (true)
        {
            Console.WriteLine("Type 'exit' to quit the application.");
            string? input = Console.ReadLine();
            if (input == null || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Cleanup();
                return 0;
            }
        }
        
        void Cleanup()
        {
        logServer.Stop();
        parser.Stop();
        Thread.Sleep(2000);
        }
    }
}