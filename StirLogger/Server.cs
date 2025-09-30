using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using StirLogger.ConfigReader;

namespace StirLogger;

public class Server
{
    public Server(BlockingCollection<LogEntry> logs, RawDataStructure rawDataStructure)
    {
        
    }

    private async void RunAsync()
    {
        // Receive data from clients and enqueue log entries
    }
    
    public void Start()
    {
        _ = Task.Run(() => RunAsync());
    }
    public void Stop()
    {
        
    }
}