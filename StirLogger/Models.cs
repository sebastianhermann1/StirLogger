using System;
using System.Collections.Concurrent;

namespace StirLogger;

public class LogEntry
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
}

public class LogManager
{
    private readonly ConcurrentQueue<LogEntry> _logQueue = new();

    public void EnqueueLog(LogEntry entry)
    {
        _logQueue.Enqueue(entry);
    }

    public bool TryDequeueLog(out LogEntry entry)
    {
        return _logQueue.TryDequeue(out entry);
    }
}
