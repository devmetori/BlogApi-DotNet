using System.Text.Json;
using BlogApi.Lib.Files.Interfaces;
using BlogApi.Shared.Enums;
using BlogApi.Shared.Models;

namespace BlogApi.Lib.Files;

public class JsonFileReader<T> : IFileReader<T>
{
    public async Task<Result<T>> ReadAsync(string path)
    {
        try
        {
            var jsonContent = await File.ReadAllTextAsync(path);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            var data = JsonSerializer.Deserialize<T>(jsonContent, options);
            if (data is null) throw new InvalidDataException("El contenido del JSON está vacío o no es válido.");
            
            return Result<T>.Success(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<T>.Failure(ERROR_CODE.UNKNOWN_ERROR, e.Message);
        }
    }
}