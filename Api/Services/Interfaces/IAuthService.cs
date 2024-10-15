using BlogApi.Data.Entity;
using BlogApi.Shared.DTOs;
using BlogApi.Shared.Models;

namespace BlogApi.Api.Services.Interfaces;

public interface IAuthService
{
    Task<Result<User>> SignUpAsync(SignUpDto model);
    Task<Result<User>> SignInAsync(SignInDto model);
    Task<Result<User>> VerifyAccountAsync(Guid id);
    Task<Result<User>> RefreshSessionAsync();
    Task<Result<User>> SignOutAsync();
    Task<Result<User>> ForgotPasswordAsync(ForgotPasswordDto model);
    Task<Result<User>> ValidateTokenAsync(string token);
    Task<Result<User>> ResetPasswordAsync(ResetPasswordDto model);
    Task<Result<User>> ChangePasswordAsync(ChangePwdDto model);
    Task<Result<User>> Verify2FacAsync(TwoFacAuthDto model);
    Task<Result<User>> Resend2FacAsync(Guid id);
    Task<Result<User>> GetUserProfile(Guid parse);
    Task<Result<IEnumerable<User>>> GetAllUsers();
}