using Microsoft.AspNetCore.Mvc;
using BlogApi.Api.Attributes;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Exceptions;
using BlogApi.Shared.DTOs.Auth;
using BlogApi.Shared.Utils;
using BlogApi.Shared.Enums;

namespace BlogApi.Api.Controllers;

[Route("auth")]
public class AuthController(IAuthService authService) : Controller
{
    private readonly string CookieName = "token";

    [HttpPost("sign-up")]
    public async Task<IActionResult> Signup([FromBody] SignUpDto model)
    {
        var result = await authService.SignUpAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> Signin([FromBody] SignInDto model)
    {
        var context = HttpContext;
        var result = await authService.SignInAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        if (result?.Data?.Tokens is not null) SetupCookie(context, result.Data.Tokens.refresh);
        return Ok(new
        {
            Success = true, result.Message, data = new { result.Data.Id, result.Data.Email }
        });
    }


    [HttpGet("verify-account/{id}")]
    public async Task<IActionResult> VerifyAccount([FromRoute] string id)
    {
        var result = await authService.VerifyAccountAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }

    [HttpGet("verify-account/resend-email/{id}")]
    public async Task<IActionResult> ResendVerifyAccountEmail([FromRoute] string id)
    {
        var result = await authService.SendVerifyAccountAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }

    [HttpPost("2fa/verify")]
    public async Task<IActionResult> Verify2FA([FromBody] TwoFacAuthDto model)
    {
        var result = await authService.Verify2FacAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        if (result.Data.Tokens is not null) SetupCookie(HttpContext, result.Data.Tokens.refresh);
        return Ok(new
        {
            Success = true, 
            result.Message, data = new
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Email = result.Data.Email,
                token = result.Data.Tokens.access,
            }
        });
    }

    [HttpGet("2fa/resend/{id}")]
    public async Task<IActionResult> Resend2Fac([FromRoute] string id)
    {
        var result = await authService.Resend2FacAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }

    [HttpGet("refresh-session")]
    public async Task<IActionResult> RefreshSession()
    {
        if (!HttpContext.Request.Cookies.TryGetValue(CookieName, out var refreshToken))
        {
            HttpException.ThrowException(ERROR_CODE.INVALID_AUTH_OPERATION, "No sea encontrado una sesión activa.");
        }

        var result = await authService.RefreshSessionAsync(refreshToken);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new
        {
            Success = true,
            result.Message,
            data = new
            {
                result.Data.Id,
                result.Data.Email,
                accessToken = result.Data.Tokens.access,
            }
        });
    }

    [IsAuthenticated]
    [HttpDelete("sign-out")]
    public async Task<IActionResult> SignOut()
    {
        var user = HttpContext.User;
        var payload = user.MapClaimsToPayload();
        var result = await authService.SignOutAsync(payload.SessionId);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        HttpContext.Response.Cookies.Delete(CookieName);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
    {
        var result = await authService.ForgotPasswordAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }

    [HttpGet("forgot-password/{token}")]
    public async Task<IActionResult> ValidateToken([FromRoute] string token)
    {
        var result = await authService.ValidateTokenAsync(token);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }

    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var result = await authService.ResetPasswordAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }

    [IsAuthenticated]
    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePwdDto model)
    {
        var result = await authService.ChangePasswordAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, result.Message, data = result.Data });
    }


    private void SetupCookie(HttpContext context, string data)
    {
        context.Response.Cookies.Append(CookieName, data, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}