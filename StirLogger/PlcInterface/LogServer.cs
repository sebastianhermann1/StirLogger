using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StirLogger.RawDataStructureReader;

namespace StirLogger.PlcInterface;

public class LogServer
{
    private CancellationToken _ctk;
    private RawDataStructure _rawDataStructure;
    public LogServer(BlockingCollection<LogEntry> logs, RawDataStructure rawDataStructure)
    {
        _ctk = CancellationToken.None;
        _rawDataStructure = rawDataStructure;
    }

    private async void RunAsync()
    {
        
    }
    
    public void Start()
    {
        _ = Task.Run(() => RunAsync());
    }
    public void Stop()
    {
        
    }
}