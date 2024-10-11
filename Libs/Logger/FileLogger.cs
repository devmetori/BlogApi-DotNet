using System.Collections.Concurrent;

namespace BlogApi.Libs.Logger;

public class FileLogger : ILogger, IDisposable
{
    private readonly string _filePath;
    private readonly int _maxFileSizeInMb;
    private readonly object _lock;
    private readonly ConcurrentQueue<string> _logQueue = new();
    private bool _isProcessing = false;


    public FileLogger(string filePath, int fileSize)
    {
        _filePath = filePath;
        _maxFileSizeInMb = fileSize;
        _lock = new object();
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);
        
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;
    private bool IsFileTooLarge()
    {
        var fileInfo = new FileInfo(_filePath);
        return fileInfo.Exists && fileInfo.Length > _maxFileSizeInMb * 1024 * 1024;
    }
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
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        var message = $"{Environment.NewLine} {DateTime.Now:yyyy-MM-dd HH:mm:ss} [{GetLogLevel(logLevel)}] {formatter(state, exception)} {Environment.NewLine}{exception}";
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
                if (_logQueue.TryDequeue(out string log))
                {
                    lock (_lock)
                    {
                        File.AppendAllText(_filePath, log);
                    }
                }
            }
            _isProcessing = false;
        });
    }

    public void Dispose()
    {
        while (!_logQueue.IsEmpty)
        {
            if (_logQueue.TryDequeue(out string log))
            {
                lock (_lock)
                {
                    File.AppendAllText(_filePath, log);
                }
            }
        }
    }
}