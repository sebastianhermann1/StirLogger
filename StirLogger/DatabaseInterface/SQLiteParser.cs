using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace StirLogger;

public class SQLiteParser
{
    private CancellationTokenSource _cts; // Changed to CancellationTokenSource
    private readonly BlockingCollection<LogEntry> _logs; // Added field to store logs

    public SQLiteParser(BlockingCollection<LogEntry> logs)
    {
        _logs = logs;
        _cts = new CancellationTokenSource(); // Initialize CancellationTokenSource
    }

    public void AddEntry(string entry)
    {
        // Implementation for adding an entry to the SQLite database
    }

    private async Task RunAsync()
    {
        try
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                // Example processing logic
                if (_logs.TryTake(out var logEntry, Timeout.Infinite, _cts.Token))
                {
                    // Process logEntry here
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
        }
    }

    public void Start()
    {
        _ = Task.Run(() => RunAsync());
    }

    public void Stop()
    {
        _cts.Cancel(); // Cancel the token
    }
}