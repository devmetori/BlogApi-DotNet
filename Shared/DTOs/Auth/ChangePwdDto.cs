namespace BlogApi.Shared.DTOs.Auth;

public record ChangePwdDto(string userId, string password, string newPassword);