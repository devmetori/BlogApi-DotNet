namespace BlogApi.Shared.DTOs.Auth;

public record ResetPasswordDto(string token, string password);