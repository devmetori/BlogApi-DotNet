namespace BlogApi.Shared.DTOs;

public record ResetPasswordDto(string token, string password);