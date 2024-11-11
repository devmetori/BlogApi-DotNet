
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Exceptions;
using BlogApi.Api.Attributes;
using BlogApi.Shared.Enums;

namespace BlogApi.Api.Controllers;


[Route("users")]
[IsAuthenticated]
public class UserController(IAuthService authService, IJwtService jwtService) : Controller
{



    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await authService.GetAllUsers();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

   
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var claims = HttpContext.User;
        var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(string.IsNullOrEmpty(userId)) HttpException.ThrowException(ERROR_CODE.UNAUTHORIZED, "Parece que ha sorgi algun error en tu sesion, por favor vuelve a iniciar sesion");

        var result = await authService.GetUserProfile(Guid.Parse(userId));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }
}