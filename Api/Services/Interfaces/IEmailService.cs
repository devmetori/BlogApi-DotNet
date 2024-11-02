using BlogApi.Shared.Models.Email;
using BlogApi.Shared.Models;

namespace BlogApi.Api.Services.Interfaces;

public interface IEmailService
{
    
    Task<Result<object>> SendEmailAsync(MailPayload mailPayload);
    Task<Result<object>> SendWelcomeEmailAsync(MailPayload mailPayload);
    Task<Result<object>> SendForgotPasswordEmailAsync(MailPayload mailPayload);
    Task<Result<object>> SendVerificationEmailAsync(MailPayload mailPayload);
    Task<Result<object>> Send2FacEmailAsync(MailPayload mailPayload);
}