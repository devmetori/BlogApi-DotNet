namespace BlogApi.Libs.Logger;

public class FileLoggerProvider(string filePath, int maxFileSizeInMb) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(filePath, maxFileSizeInMb);
    }

    public void Dispose()
    {
  
    }
}