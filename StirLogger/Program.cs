using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using StirLogger.ConfigReader;


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
        Server server = new Server(logs, rawDataStructure);
        SQLiteParser parser = new SQLiteParser(logs);

        // Start tasks
        server.Start();
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
        server.Stop();
        parser.Stop();
        Thread.Sleep(2000);
        }
    }
}