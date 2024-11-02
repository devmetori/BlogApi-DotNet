using BlogApi.Shared.Models;

namespace BlogApi.Lib.Files.Interfaces;

public interface IFileReader<T>
{
    Task<Result<T>> ReadAsync(string path);
}