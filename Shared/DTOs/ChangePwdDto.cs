namespace BlogApi.Shared.DTOs;

public record ChangePwdDto(string userId, string password, string newPassword);