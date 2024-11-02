using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BlogApi.Api.Attributes;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Exceptions;
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

   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserProfile([FromRoute] string id)
    {
        var result = await authService.GetUserProfile(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }
}