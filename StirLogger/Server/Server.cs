using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StirLogger.Server;

public class Server
{
    public Server(BlockingCollection<LogEntry> logs)
    {
        
    }

    private async void RunAsync()
    {
        
    }
    
    public void start()
    {
        _ = Task.Run(() => RunAsync());
    }
}