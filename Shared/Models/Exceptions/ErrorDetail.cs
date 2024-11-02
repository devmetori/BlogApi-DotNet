namespace BlogApi.Shared.Models.Exceptions;

public class ErrorDetail
{
    public int Code { get; set; } = 0;
    public string Message { get; set; }
}