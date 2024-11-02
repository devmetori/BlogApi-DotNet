using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Lib.TemplateEngine.Interfaces;
using BlogApi.Shared.Enums;
using BlogApi.Shared.Models;
using BlogApi.Shared.Models.Email;
using BlogApi.Shared.Models.Settings;

namespace BlogApi.Api.Services;

public class EmailService : IEmailService
{
    private readonly ITemplateService _templateService;
    private readonly ILogger<EmailService> _logger;
    private readonly MailSettings _mailSettings;
    private readonly  IWebHostEnvironment _env;

    public EmailService(ILogger<EmailService> logger, IOptions<MailSettings> mailSettings,
        ITemplateService templateService, IWebHostEnvironment env)
    {
        this._logger = logger;
        this._mailSettings = mailSettings.Value;
        this._templateService = templateService;
        this._env = env;
    }

    private BodyBuilder SetupMailBody(MailPayload mailPayload)
    {
        var builder = new BodyBuilder();
        if (mailPayload?.Attachments is not null && mailPayload.Attachments.Count > 0)
        {
            foreach (var file in mailPayload.Attachments)
            {
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            }
        }

        builder.HtmlBody = mailPayload?.Body;
        return builder;
    }

    private MimeMessage FormatMail(MailPayload mailPayload)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        email.To.Add(MailboxAddress.Parse(mailPayload.ToEmail));
        email.Subject = mailPayload.Subject;
        var builder = SetupMailBody(mailPayload);
        email.Body = builder.ToMessageBody();
        return email;
    }

    private async Task<Result<object>> CreateMailTransactionAsync(MimeMessage email)
    {
        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            if (!client.IsConnected)
            {
                return Result<object>.Failure(ERROR_CODE.SERVICE_UNAVAILABLE,
                    "No se pudo establecer la conexión con el servidor SMTP");
            }

            await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            if (!client.IsAuthenticated)
            {
                return Result<object>.Failure(ERROR_CODE.SERVICE_UNAVAILABLE,
                    "No se pudo autenticar con el servidor SMTP");
            }

            await client.SendAsync(email);
            await client.DisconnectAsync(true);
            _logger.LogInformation("Email sent successfully to {Recipient}", email.To.ToString());
            return Result<object>.Success("Correo enviado correctamente");
        }
        catch (AuthenticationException ex)
        {
            _logger.LogError(ex, "Authentication Error: {Message}", ex.Message);
            return Result<object>.Failure(ERROR_CODE.SERVICE_UNAVAILABLE, "Error de autenticación");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result<object>.Failure(ERROR_CODE.SERVICE_UNAVAILABLE, "Error de autenticación");
        }
    }

    public async Task<Result<object>> SendEmailAsync(MailPayload mailPayload)
    {
        if(_env.IsDevelopment())
        {
            _logger.LogInformation("Enviando correo a {Recipient} con el asunto: {Subject}", mailPayload.ToEmail, mailPayload.Subject);
            return Result<object>.Success("Correo enviado correctamente");
        }
        
        if (_mailSettings is null) throw new NullReferenceException("Mail settings not found");
        var email = FormatMail(mailPayload);
        var result = await CreateMailTransactionAsync(email);
        return !result.IsSuccess ? Result<object>.Failure(result.Code, result.Message) : Result<object>.Success("Correo enviado correctamente");
    }


    public async Task<Result<object>> SendWelcomeEmailAsync(MailPayload mailPayload)
    {
        mailPayload.Body = await _templateService.GetTemplateAsync("VerifyAccountTemplate", mailPayload.Model);
        var result = await SendEmailAsync(mailPayload);
        if (!result.IsSuccess) return Result<object>.Failure(result.Code, result.Message);
        return Result<object>.Success(result.Message);
    }

    public async Task<Result<object>> SendForgotPasswordEmailAsync(MailPayload mailPayload)
    {
        mailPayload.Body = await _templateService.GetTemplateAsync("VerifyAccountTemplate", mailPayload.Model);
        var result = await SendEmailAsync(mailPayload);
        if (!result.IsSuccess) return Result<object>.Failure(result.Code, result.Message);
        return Result<object>.Success(result.Message);
    }

    public async Task<Result<object>> SendVerificationEmailAsync(MailPayload mailPayload)
    {
        mailPayload.Body = await _templateService.GetTemplateAsync("VerifyAccountTemplate", mailPayload.Model);
        var result = await SendEmailAsync(mailPayload);
        if (!result.IsSuccess) return Result<object>.Failure(result.Code, result.Message);
        return Result<object>.Success(result.Message);
    }

    public async Task<Result<object>> Send2FacEmailAsync(MailPayload mailPayload)
    {
        mailPayload.Body = await _templateService.GetTemplateAsync("VerifyAccountTemplate", mailPayload.Model);
        var result = await SendEmailAsync(mailPayload);
        if (!result.IsSuccess) return Result<object>.Failure(result.Code, result.Message);
        return Result<object>.Success(result.Message);
    }
}