using System;
using System.Collections.Concurrent;
using System.IO;
using StirLogger.ConfigReader;


namespace StirLogger;

class Program
{
    static void Main(string[] args)
    {
        DataStructureReader reader = new DataStructureReader(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StirLogger",
                "datastructure"));
        
        RawDataStructure rawDataStructure;
        
        BlockingCollection<LogEntry> logs = new BlockingCollection<LogEntry>();
        SQLiteParser parser = new SQLiteParser(logs);
        Server.Server server = new Server.Server(logs);

        // start tasks
        server.start();
        parser.start();
        
        while (true)
        {
            Console.WriteLine("Type 'exit' to quit the application.");
            string? input = Console.ReadLine();
            if (input == null || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Cleanup();
                break;
            }
        }
    }
    
    static void Cleanup()
    {
        
    }
}
