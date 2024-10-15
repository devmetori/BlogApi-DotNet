using System.Security.Claims;
using BlogApi.Shared.Models;

namespace BlogApi.Api.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken(string token,  IEnumerable<Claim> claims);
    JwtValidation IsValidToken(string token);
}