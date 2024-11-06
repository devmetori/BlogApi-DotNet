using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Models.Settings;
using BlogApi.Shared.Models.Auth;


namespace BlogApi.Api.Services;

public class JwtService : IJwtService
{
    private readonly ILogger<JwtService> _logger;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public JwtService(ILogger<JwtService> logger, IOptions<JwtSettings> jwtSettings)
    {
        this._logger = logger;
        this._jwtSettings = jwtSettings;
    }

    private JwtSettings GetJwtSettings()
    {
        var jwtSettings = _jwtSettings.Value;
        if (jwtSettings is null) throw new NullReferenceException("JwtSettings is null");
        return jwtSettings;
    }

    private IEnumerable<Claim> GeneratePayload(JwtPayload payload)
    {
        return new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, payload.Id),
            new(ClaimTypes.Role, payload.Role.ToString()),
            new("sessionId", payload.SessionId),
        };
    }

    public JwtValidation IsValidToken(string token, string? SecretKey = null)
    {
        var jwtSettings = GetJwtSettings();
        if (string.IsNullOrEmpty(token)) return JwtValidation.Failure("Token no puede ser nulo o vacío.");
        var key = Encoding.ASCII.GetBytes(SecretKey ?? jwtSettings.SecretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        var result = new JsonWebTokenHandler().ValidateToken(token, validationParameters);

        if (result.IsValid && result.ClaimsIdentity != null)
        {
            return JwtValidation.Success(new ClaimsPrincipal(result.ClaimsIdentity));
        }

        return JwtValidation.Failure("Token no es válido o ha expirado");
    }


    private SigningCredentials SigningCredentials(string SecretKey)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    private string GenerateSingleToken(IEnumerable<Claim> claims, SigningCredentials credentials, int Expiration)
    {
        var jwtSettings = GetJwtSettings();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer= jwtSettings.Issuer,
            IssuedAt = DateTime.UtcNow,
            Audience= jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Expiration),
            SigningCredentials = credentials,
        };

        return  new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }


    public (string access, string refresh) GenerateTokens(JwtPayload payload)
    {
        var access = GenerateAccessToken(payload);
        var refresh = GenerateRefreshToken(payload);
        return (access, refresh);
    }

    public string GenerateAccessToken(JwtPayload payload)
    {
        var jwtSettings = GetJwtSettings();
        var credentials = SigningCredentials(jwtSettings.SecretKey);
        var claims = GeneratePayload(payload);
        return GenerateSingleToken(claims, credentials, jwtSettings.Expiration);
    }

    public string GenerateToken(IEnumerable<Claim> claims, string SecretKey)
    {
        var jwtSettings = GetJwtSettings();
        var credentials = SigningCredentials(SecretKey);
        return GenerateSingleToken(claims, credentials, jwtSettings.Expiration);
    }

    public string GenerateRefreshToken(JwtPayload payload)
    {
        var jwtSettings = GetJwtSettings();
        var credentials = SigningCredentials(jwtSettings.SecretKey);
        var claims = GeneratePayload(payload);
        var expiration = jwtSettings.Expiration * 60 * 24 * 7;
        return GenerateSingleToken(claims, credentials, expiration);
    }
}