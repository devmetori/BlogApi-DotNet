using BlogApi.Api.Services.Interfaces;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.Models;
using BlogApi.Data.Entity;
using BlogApi.Shared.DTOs;

namespace BlogApi.Api.Services;
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;

    public AuthService(IUserRepository userRepository, ISessionRepository sessionRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
    }

    public Task<Result<User>> SignUpAsync(SignUpDto model)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> SignInAsync(SignInDto model)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> VerifyAccountAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> RefreshSessionAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> SignOutAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> ForgotPasswordAsync(ForgotPasswordDto model)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> ValidateTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> ResetPasswordAsync(ResetPasswordDto model)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> ChangePasswordAsync(ChangePwdDto model)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> Verify2FacAsync(TwoFacAuthDto model)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> Resend2FacAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> GetUserProfile(Guid parse)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<User>>> GetAllUsers()
    {
        return await _userRepository.GetAllAsync();
    }
}