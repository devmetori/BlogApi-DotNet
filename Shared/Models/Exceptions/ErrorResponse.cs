namespace BlogApi.Shared.Models.Exceptions;

public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public ErrorDetail Error { get; set; }
}