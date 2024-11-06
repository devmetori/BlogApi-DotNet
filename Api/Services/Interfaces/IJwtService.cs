using System.Security.Claims;
using BlogApi.Shared.Models.Auth;

namespace BlogApi.Api.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(JwtPayload payload);
    string GenerateToken(IEnumerable<Claim> payload, string SecretKey);
    string GenerateRefreshToken(JwtPayload payload);
    JwtValidation IsValidToken(string token, string? SecretKey = null);

    (string access, string refresh) GenerateTokens(JwtPayload payload);
}