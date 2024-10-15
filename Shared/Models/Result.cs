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

    private Result(bool isSuccess, T data)
    {
        IsSuccess = isSuccess;
        Message = "Operation successful";
        Data = data;
    }

    public static Result<T> Success(T data = default(T)) => new Result<T>(true, data);
    public static Result<T> Failure(ERROR_CODE code, string message) => new Result<T>(false, code, message);
}