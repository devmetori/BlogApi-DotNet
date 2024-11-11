using System.Collections.Concurrent;

namespace BlogApi.Lib.Logger;

public class FileLogger : ILogger, IDisposable
{
    private readonly string _baseFilePath;
    private readonly object _lock = new object();
    private readonly ConcurrentQueue<string> _logQueue = new();
    private bool _isProcessing = false;
    private string _currentFilePath;
    private DateTime _currentDate;

    public FileLogger(string baseFilePath)
    {
        _baseFilePath = baseFilePath;
        _currentDate = DateTime.Now;
        _currentFilePath = GetCurrentFilePath();
        var directory = Path.GetDirectoryName(_baseFilePath);


        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (File.Exists(_currentFilePath)) return;
        using (File.Create(_currentFilePath))
        {
        }
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    private string GetLogLevel(LogLevel level) => level switch
    {
        LogLevel.Trace => "TRACE",
        LogLevel.Debug => "DEBUG",
        LogLevel.Information => "INFO",
        LogLevel.Warning => "WARN",
        LogLevel.Error => "ERROR",
        LogLevel.Critical => "FATAL",
        _ => "INFO"
    };

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message =
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{GetLogLevel(logLevel)}] {formatter(state, exception)} {Environment.NewLine}{exception}";
        _logQueue.Enqueue(message);
        ProcessLogQueue();
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    private void ProcessLogQueue()
    {
        if (_isProcessing) return;
        _isProcessing = true;
        Task.Run(() =>
        {
            while (!_logQueue.IsEmpty)
            {
                if (!_logQueue.TryDequeue(out string log)) continue;
                lock (_lock)
                {
                    File.AppendAllText(_currentFilePath, log);
                }
            }

            _isProcessing = false;
        });
    }

  
    

    private string GetCurrentFilePath()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd");
        return Path.Combine(Path.GetDirectoryName(_baseFilePath),
            $"{Path.GetFileNameWithoutExtension(_baseFilePath)}-{timestamp}{Path.GetExtension(_baseFilePath)}");
    }

   

    public void Dispose()
    {
        while (!_logQueue.IsEmpty)
        {
            if (!_logQueue.TryDequeue(out string log)) continue;

            lock (_lock)
            {
                File.AppendAllText(_currentFilePath, log);
            }
        }
    }
}

