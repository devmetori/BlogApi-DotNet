using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Models;

namespace BlogApi.Api.Services;
public class JwtService(IConfiguration config, ILogger<JwtService> logger) : IJwtService
{
    private JwtSettings GetJwtSettings()
    {
        var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings is null) throw new NullReferenceException("JwtSettings is null");
  
        return jwtSettings;
    }

    public JwtValidation IsValidToken(string token)
    {
        var jwtSettings = GetJwtSettings();
        if (string.IsNullOrEmpty(token)) return JwtValidation.Failure("Token no puede ser nulo o vacío.");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

        try
        {
            TokenValidationParameters validator = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validator, out SecurityToken validatedToken);
            if (validatedToken is JwtSecurityToken jwtToken)
            {
                if (jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase)) return JwtValidation.Success(principal);
            }
        }
        catch (SecurityTokenExpiredException e)
        {
            logger.LogError(e, "Token ha expirado.");
            return JwtValidation.Failure("Token ha expirado.");
        }
        catch (SecurityTokenException e)
        {
            logger.LogError(e, "Token ha expirado.");
            return JwtValidation.Failure("Token no es válido.");
        }

        return JwtValidation.Failure("Token no es válido o ha expirado");
    }

    private SigningCredentials SigningCredentials(string SecretKey)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private string GenerateToken(JwtSettings jwtSettings, IEnumerable<Claim> claims, SigningCredentials credentials)
    {
        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtSettings.Expiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var jwtSettings = GetJwtSettings();
        var credentials = SigningCredentials(jwtSettings.SecretKey);
       
        return GenerateToken(jwtSettings, claims, credentials);
    }

    public string GenerateRefreshToken(string token, IEnumerable<Claim> claims)
    {
        var result = IsValidToken(token);
        if (!result.IsValid) throw new SecurityTokenException(result.Message);
        
        var jwtSettings = GetJwtSettings();
        var credentials = SigningCredentials(jwtSettings.SecretKey);
         jwtSettings.Expiration = 60 * 24 * 7;
        return GenerateToken(jwtSettings, claims, credentials);
    }
}