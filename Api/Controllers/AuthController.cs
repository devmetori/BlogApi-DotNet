using Microsoft.AspNetCore.Mvc;
using BlogApi.Api.Attributes;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Exceptions;
using BlogApi.Shared.DTOs;

namespace BlogApi.Api.Controllers;

[Route("auth")]
[ApiController]
public class AuthController(IAuthService authService) : Controller
{
        
    

    [HttpPost("sign-up")]
    public  async Task<IActionResult> Signup([FromBody] SignUpDto model)
    {
        var result = await authService.SignUpAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPost("sign-in")]
    public  async Task<IActionResult> Signin([FromBody] SignInDto model)
    {
        var result = await authService.SignInAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("verify-account/{id}")]
    public  async Task<IActionResult> VerifyAccount([FromRoute] string id)
    {
        var result = await authService.VerifyAccountAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("verify-account/resend-email/{id}")]
    public  async Task<IActionResult> ResendVerifyAccountEmail([FromRoute] string id)
    {
        var result = await authService.VerifyAccountAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPost("2fa/verify")]
    public  async Task<IActionResult> Verify2FA([FromBody] TwoFacAuthDto model)
    {
        var result = await authService.Verify2FacAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("2fa/resend/{id}")]
    public  async Task<IActionResult> Resend2Fac([FromRoute] string id)
    {
        var result = await authService.Resend2FacAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("refresh-session")]
    public  async Task<IActionResult> RefreshSession()
    {
        var result = await authService.RefreshSessionAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpDelete("sign-out")]
    public  async Task<IActionResult> Signout()
    {
        var result = await authService.SignOutAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPost("forgot-password")]
    public  async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        var result = await authService.ForgotPasswordAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("forgot-password/{token}")]
    public  async Task<IActionResult> ValidateToken([FromRoute] string token)
    {
        var result = await authService.ValidateTokenAsync(token);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPatch("reset-password")]
    public  async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var result = await authService.ResetPasswordAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }
    [IsAuthenticated]
    [HttpPatch("change-password")]
    public  async Task<IActionResult> ChangePassword([FromBody] ChangePwdDto model)
    {
        var result = await authService.ChangePasswordAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }
}