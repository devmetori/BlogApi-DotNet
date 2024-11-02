using BlogApi.Shared.Models;

namespace BlogApi.Api.Services.Interfaces;

public interface ITwoFactorAuthService
{
    (string code, string secret) GenerateCode();
   bool  IsValidCode(string secret, string code);
    
}