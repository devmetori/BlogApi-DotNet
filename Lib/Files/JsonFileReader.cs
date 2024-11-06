using System.Text.Json;
using System.Text.Json.Serialization;
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
    
public async Task<Result<object>> WriteAsync(string path, T data)
{
    try
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        var jsonContent = JsonSerializer.Serialize(data, options);
        await File.WriteAllTextAsync(path, jsonContent);

        return Result<object>.Success("Fichero JSON creado correctamente.");
    }
    catch (IOException ioEx)
    {
        Console.WriteLine(ioEx);
        return Result<object>.Failure(ERROR_CODE.IO_ERROR, ioEx.Message);
    }
    catch (UnauthorizedAccessException uaEx)
    {
        Console.WriteLine(uaEx);
        return Result<object>.Failure(ERROR_CODE.FILE_SYSTEM_UNAUTHORIZED_ACCESS, uaEx.Message);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Result<object>.Failure(ERROR_CODE.UNKNOWN_ERROR, e.Message);
    }
}   
}