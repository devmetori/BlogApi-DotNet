namespace BlogApi.Lib.Logger;

public class FileLoggerProvider(string filePath) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(filePath);
    }

    public void Dispose()
    {
  
    }
}