using BlogApi.Shared.Enums;

namespace BlogApi.Shared.Models;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public ERROR_CODE Code { get; private set; }
    public T? Data { get; private set; }

    private Result(bool isSuccess, ERROR_CODE code, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
        Code = code;
    }

    private Result(bool isSuccess, string message, T data)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    private Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result<T> Success(T data = default(T)) => new(true, "Operation successful", data);
    public static Result<T> Success(string message, T data = default(T)) => new(true, message, data);
    public static Result<T> Success(string message) => new(true, message);
    public static Result<T> Failure(ERROR_CODE code, string message) => new(false, code, message);
}