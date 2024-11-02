using BlogApi.Data.Entity;
using BlogApi.Shared.DTOs.Auth;
using BlogApi.Shared.Models;

namespace BlogApi.Api.Services.Interfaces;

public interface IAuthService
{
    Task<Result<UserDto>> SignUpAsync(SignUpDto model);
    Task<Result<User>> VerifyAccountAsync(Guid id);
    Task<Result<User>> SendVerifyAccountAsync(Guid id);
    Task<Result<UserDto>> SignInAsync(SignInDto model);
    Task<Result<UserDto>> RefreshSessionAsync(string token);
    Task<Result<User>> SignOutAsync(string sessionId);
    Task<Result<User>> ForgotPasswordAsync(ForgotPasswordDto model);
    Task<Result<User>> ValidateTokenAsync(string token);
    Task<Result<User>> ResetPasswordAsync(ResetPasswordDto model);
    Task<Result<User>> ChangePasswordAsync(ChangePwdDto model);
    Task<Result<UserDto>> Verify2FacAsync(TwoFacAuthDto model);
    Task<Result<User>> Resend2FacAsync(Guid id);
    Task<Result<User>> GetUserProfile(Guid parse);
    Task<Result<IEnumerable<User>>> GetAllUsers();
}