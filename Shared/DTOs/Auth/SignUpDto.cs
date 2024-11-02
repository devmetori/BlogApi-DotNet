namespace BlogApi.Shared.DTOs.Auth;

public record SignUpDto(string name,string surname, string email, string password);