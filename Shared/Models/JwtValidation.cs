using System.Security.Claims;

namespace BlogApi.Shared.Models;

public class JwtValidation
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public ClaimsPrincipal? Claims { get; set; }

    public static JwtValidation Success(ClaimsPrincipal claims){
       return  new() { IsValid = true, Message = "Token is valid", Claims = claims };
    }

    public static JwtValidation Failure(string message) => new() { IsValid = false, Message = message, Claims = null };
}