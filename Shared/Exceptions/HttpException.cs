using BlogApi.Shared.Enums;

namespace BlogApi.Shared.Exceptions;

public class HttpException(int statusCode, ERROR_CODE code, string message) : Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
    public ERROR_CODE Code { get; } = code;
}

