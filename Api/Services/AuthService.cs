using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.Models;
using BlogApi.Data.Entity;
using BlogApi.Shared.DTOs.Auth;
using BlogApi.Shared.Enums;
using BlogApi.Shared.Models.Auth;
using BlogApi.Shared.Models.Email;
using BlogApi.Shared.Utils;

namespace BlogApi.Api.Services;

public class AuthService : IAuthService
{
    private readonly ITwoFactorAuthService _twoFactorAuthService;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly IWebHostEnvironment _env;


    public AuthService(IWebHostEnvironment env,
        IConfiguration configuration, IUserRepository userRepository, IRoleRepository roleRepository,
        ISessionRepository sessionRepository, IJwtService jwtService,
        IEmailService emailService, ITwoFactorAuthService twoFactorAuthService)
    {
        _twoFactorAuthService = twoFactorAuthService;
        _sessionRepository = sessionRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _configuration = configuration;
        _emailService = emailService;
        _jwtService = jwtService;
        _env = env;
    }

    public async Task<Result<UserDto>> SignUpAsync(SignUpDto model)
    {
        var fetchResult = await _userRepository.FindFirstAsync(m => m.Email == model.email);

        if (fetchResult is { IsSuccess: true, Data: not null })
        {
            return Result<UserDto>.Failure(ERROR_CODE.CONFLICT_OPERATION, "El correo electrónico ya está registrado");
        }


        var RoleFetchResult = await _roleRepository.FindFirstAsync(role =>
            role.Name == RoleName.READER.ToString() && role.Code == RoleName.READER);
        if (!RoleFetchResult.IsSuccess)
        {
            return Result<UserDto>.Failure(fetchResult.Code,
                "No se ha podido registrar el usuario, intentalo de nuevo");
        }

        var hashedPassword = new PasswordHasher<object>().HashPassword(null, model.password);

        var result = await _userRepository.AddAsync(new User
        {
            Email = model.email,
            AccountVerified = false,
            Password = hashedPassword,
            Name = model.name,
            Surname = model.surname,
            RoleId = RoleFetchResult.Data.Id,
        });

        if (!result.IsSuccess) Result<User>.Failure(result.Code, result.Message);
        MailPayload Payload = new()
        {
            ToEmail = model.email, Subject = "Bienvenido a BlogAPI",
            Model = new TemplateModel(model.name, "http://localhost:5000/blog")
        };
        var resultWelcomeEmail = await _emailService.SendWelcomeEmailAsync(Payload);
        if (!resultWelcomeEmail.IsSuccess)
        {
            return Result<UserDto>.Failure(resultWelcomeEmail.Code, resultWelcomeEmail.Message);
        }

        MailPayload verificationPayload = new()
        {
            ToEmail = model.email, Subject = "Verifica tu cuenta",
            Model = new TemplateModel(model.name, "http://localhost:5000/auth/verify/" + result.Data.Id)
        };
        var resultVerificationEmail = await _emailService.SendEmailAsync(verificationPayload);
        if (!resultVerificationEmail.IsSuccess)
        {
            Result<UserDto>.Failure(resultVerificationEmail.Code, resultVerificationEmail.Message);
        }

        return Result<UserDto>.Success("Usuario registrado exitosamente");
    }

    public async Task<Result<User>> VerifyAccountAsync(Guid id)
    {
        var result = await _userRepository.FindFirstAsync(u => u.Id == id);

        if (!result.IsSuccess) return Result<User>.Failure(result.Code, result.Message);
        var user = result.Data;
        if (user.AccountVerified)
        {
            return Result<User>.Failure(ERROR_CODE.CONFLICT_OPERATION, "La cuenta se sea verificado anteriorment");
        }

        user.AccountVerified = true;
        var updateResult = await _userRepository.UpdateAsync(user);
        return !updateResult.IsSuccess
            ? Result<User>.Failure(updateResult.Code, "No se ha podido verificar la cuenta, intenta de nuevo")
            : Result<User>.Success("Cuenta verificada correctamente");
    }

    public async Task<Result<User>> SendVerifyAccountAsync(Guid id)
    {
        var result = await _userRepository.FindFirstAsync(m => m.Id == id);

        if (!result.IsSuccess) return Result<User>.Failure(result.Code, result.Message);
        var user = result.Data;
        if (user.AccountVerified)
        {
            return Result<User>.Failure(ERROR_CODE.CONFLICT_OPERATION, "La cuenta se sea verificado anteriorment");
        }

        MailPayload verificationPayload = new()
        {
            ToEmail = user.Email,
            Subject = "Verifica tu cuenta",
            Model = new TemplateModel(user.Name, "http://localhost:5000/auth/verify/" + user.Id)
        };
        var resultVerificationEmail = await _emailService.SendVerificationEmailAsync(verificationPayload);
        return !resultVerificationEmail.IsSuccess
            ? Result<User>.Failure(resultVerificationEmail.Code, resultVerificationEmail.Message)
            : Result<User>.Success("Correo enviado coorrectamente");
    }

    public async Task<Result<UserDto>> SignInAsync(SignInDto model)
    {
        var result = await _userRepository.FindFirstAsync(user => user.Email == model.email);
        if (!result.IsSuccess)
        {
            return Result<UserDto>.Failure(result.Code,
                "El usuario no se ha encontrado, asegurate de estar registrado o de haber ingresado correctamente los datos");
        }

        var passwordVerification =
            new PasswordHasher<object>().VerifyHashedPassword(null, result.Data.Password, model.password);
        var isValid = passwordVerification == PasswordVerificationResult.Success;
        if (!isValid) return Result<UserDto>.Failure(ERROR_CODE.UNAUTHORIZED, "Las credenciales no son válidas");

        if (!result.Data.Enabled2fa)
        {
            var createResult = await _sessionRepository.CreateNewSession(result.Data.Id);

            if (!createResult.IsSuccess) return Result<UserDto>.Failure(createResult.Code, createResult.Message);


            var NewPayload = new JwtPayload()
            {
                Id = result.Data.Id.ToString(),
                Role = (int)result.Data.Role.Code,
                SessionId = result.Data.Session.Id.ToString()
            };
            //TODO: Asegurarse de que el tiempo de expiración el refresh token sea correcto
            var (access, refresh) = _jwtService.GenerateTokens(NewPayload);
            return Result<UserDto>.Success(result.Data.MapToDto(new JwtDto(access, refresh)));
        }

        var (code, secret) = _twoFactorAuthService.GenerateCode();

        if (_env.IsDevelopment())
        {
            Console.WriteLine($"[Code] =====================>[{code}]");
        }

        var payload = new MailPayload
        {
            ToEmail = result.Data.Email,
            Subject = "Código de verificación",
            Model = new TemplateModel(result.Data.Name, code)
        };
        var emailResult = await _emailService.Send2FacEmailAsync(payload);
        if (!emailResult.IsSuccess)
        {
            return Result<UserDto>.Failure(emailResult.Code, emailResult.Message);
        }

        var user = result.Data;
        user.Verified2fa = false;
        user.Code2fa = code;
        user.Secret2fa = secret;
        var updateResult = await _userRepository.UpdateAsync(user);


        return !updateResult.IsSuccess
            ? Result<UserDto>.Failure(updateResult.Code, updateResult.Message)
            : Result<UserDto>.Success(
                "Se ha enviado un código de verificación a tu correo electrónico, revisa tu bandeja de entrada",
                updateResult.Data.MapToDto());
    }

    public async Task<Result<UserDto>> Verify2FacAsync(TwoFacAuthDto model)
    {
        var result = await _userRepository.FindUserWithRoleAsync(user => user.Id == Guid.Parse(model.id));
        if (!result.IsSuccess)
        {
            return Result<UserDto>.Failure(result.Code,
                "Usuario no encontrado, asegurate de estar registrado o de haber ingresado correctamente los datos");
        }

        if (result.Data.Verified2fa)
        {
            return Result<UserDto>.Failure(ERROR_CODE.CONFLICT_OPERATION, "El usuario ya ha sido verificado");
        }

        var isValid = _twoFactorAuthService.IsValidCode(result.Data.Secret2fa, model.code);
        if (!isValid)
        {
            return Result<UserDto>.Failure(ERROR_CODE.UNAUTHORIZED, "El código ingresado no es válido");
        }

        var createResult = await _sessionRepository.CreateNewSession(result.Data.Id);

        if (!createResult.IsSuccess)
        {
            return Result<UserDto>.Failure(createResult.Code,
                "Ha sorgido un error durante la creación de la sesión, vuelve a intentarlo de nuevo");
        }

        var NewPayload = new JwtPayload()
        {
            Id = result.Data.Id.ToString(),
            Role = (int)result.Data.Role.Code,
            SessionId = createResult.Data.Id.ToString()
        };

        var (access, refresh) = _jwtService.GenerateTokens(NewPayload);

        result.Data.Verified2fa = true;
        result.Data.Secret2fa = null;
        result.Data.Code2fa = null;

        var updateResult = await _userRepository.UpdateAsync(result.Data);

        if (!updateResult.IsSuccess) return Result<UserDto>.Failure(updateResult.Code, updateResult.Message);

        var data = result.Data.MapToDto(new JwtDto(access, refresh));
        return Result<UserDto>.Success(data);
    }

    public async Task<Result<User>> Resend2FacAsync(Guid id)
    {
        var result = await _userRepository.FindFirstAsync(u => u.Id == id);
        if (!result.IsSuccess) return Result<User>.Failure(result.Code, result.Message);
        if (result.Data.Verified2fa)
        {
            return Result<User>.Failure(ERROR_CODE.CONFLICT_OPERATION, "El usuario ya ha sido verificado");
        }

        var payload = new MailPayload
        {
            ToEmail = result.Data.Email,
            Subject = "Código de verificación",
            Model = new TemplateModel(result.Data.Name, result.Data.Code2fa)
        };
        var emailResult = await _emailService.Send2FacEmailAsync(payload);

        if (!emailResult.IsSuccess) return Result<User>.Failure(emailResult.Code, emailResult.Message);

        return Result<User>.Success("Codigo de verificacion reenviado correctamente");
    }

    public async Task<Result<UserDto>> RefreshSessionAsync(string token)
    {
        var result = _jwtService.IsValidToken(token);


        if (!result.IsValid) return Result<UserDto>.Failure(ERROR_CODE.INVALID_TOKEN, result.Message);
        var payload = result.Claims.MapClaimsToPayload();
        var fetchResult =
            await _userRepository.FindUserWithSessionAndRoleAsync(user => user.Id == Guid.Parse(payload.Id));
        if (!fetchResult.IsSuccess) return Result<UserDto>.Failure(fetchResult.Code, fetchResult.Message);
        var user = fetchResult.Data;
        if (!user.Verified2fa && user.Verified2fa)
            return Result<UserDto>.Failure(ERROR_CODE.UNAUTHORIZED, "Usuario no verificado");

        var isSessionStillActive = user.Session.IsActive && user.Session.ExpiresAt > DateTime.Now;
        if (!isSessionStillActive)
            return Result<UserDto>.Failure(ERROR_CODE.INVALID_AUTH_OPERATION, "No se ha encontrado una sesión activa");

        var NewPayload = new JwtPayload()
        {
            Id = user.Id.ToString(),
            Role = (int)user.Role.Code,
            SessionId = user.Session.Id.ToString()
        };
        var accessToken = _jwtService.GenerateAccessToken(NewPayload);

        return Result<UserDto>.Success(user.MapToDto(new JwtDto(access: accessToken, refresh: token)));
    }

    public async Task<Result<User>> SignOutAsync(string sessionId)
    {
        var sessionResult = await _sessionRepository.FindFirstAsync(session => session.Id == Guid.Parse(sessionId));
        if (!sessionResult.IsSuccess)
        {
            return Result<User>.Failure(sessionResult.Code, "Actualemente no hemos encontrado una sesión activa");
        }

        var deleteResult = await _sessionRepository.DeleteAsync(sessionResult.Data);
        return !deleteResult.IsSuccess
            ? Result<User>.Failure(deleteResult.Code, deleteResult.Message)
            : Result<User>.Success("Sesión cerrada correctamente");
    }

    public async Task<Result<User>> ForgotPasswordAsync(ForgotPasswordDto model)
    {
        var result = await _userRepository.FindFirstAsync(user => user.Email == model.email);

        if (!result.IsSuccess) return Result<User>.Failure(result.Code, result.Message);
        var user = result.Data;

        if (!user.AccountVerified)
        {
            return Result<User>.Failure(ERROR_CODE.INVALID_AUTH_OPERATION,
                "Tu cuenta no ha sido verificada, verifica tu cuenta para poder restablecer tu contraseña");
        }

        if (user.RestoreAttemptAt.HasValue)
        {
            var timeSinceLastRequest = (DateTime.UtcNow - user.RestoreAttemptAt.Value).TotalMinutes;
            var delayTime = 5 * (Math.Pow(2, user.RestoreAttemptCount) - 1);

            if (timeSinceLastRequest < delayTime)
            {
                return Result<User>.Failure(ERROR_CODE.INVALID_OPERATION,
                    $"Debes esperar {delayTime} minutos antes volver de solicitar un nuevo cambio de contraseña");
            }
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        var SecretKey = _configuration.GetValue<string>("SecretResetPassword");
        var token = _jwtService.GenerateToken(claims, SecretKey);

        user.PasswordToken = token;
        user.RestoreAttemptAt = DateTime.UtcNow;
        user.RestoreAttemptCount += 1;

        var updateResult = await _userRepository.UpdateAsync(user);
        if (!updateResult.IsSuccess) return Result<User>.Failure(updateResult.Code, updateResult.Message);

        var FrontEndUrl = _configuration.GetValue<string>("ForgotPasswordUrl");
        MailPayload payload = new()
        {
            ToEmail = user.Email,
            Subject = "Restablecer contraseña",
            Model = new TemplateModel(user.Name, $"{FrontEndUrl}/{token}")
        };
        var emailResult = await _emailService.SendForgotPasswordEmailAsync(payload);
        if (!emailResult.IsSuccess) return Result<User>.Failure(emailResult.Code, emailResult.Message);
        return Result<User>.Success(
            "Se ha enviado un correo con las instrucciones para restablecer tu contraseña, revisa tu correo");
    }

    public async Task<Result<User>> ValidateTokenAsync(string token)
    {
        var SecretKey = _configuration.GetValue<string>("SecretResetPassword");
        var result = _jwtService.IsValidToken(token, SecretKey);
        if (!result.IsValid) return Result<User>.Failure(ERROR_CODE.INVALID_TOKEN, result.Message);
        var Id = result.Claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Id is null)
        {
            return Result<User>.Failure(ERROR_CODE.INVALID_TOKEN, "No se ha encontrado el identificador del usuario");
        }

        var fetchResult = await _userRepository.FindFirstAsync(user => user.Id == Guid.Parse(Id));
        return !fetchResult.IsSuccess
            ? Result<User>.Failure(fetchResult.Code, fetchResult.Message)
            : Result<User>.Success("Token válido");
    }

    public async Task<Result<User>> ResetPasswordAsync(ResetPasswordDto model)
    {
        var SecretKey = _configuration.GetValue<string>("SecretResetPassword");
        var result = _jwtService.IsValidToken(model.token, SecretKey);
        if (!result.IsValid) return Result<User>.Failure(ERROR_CODE.INVALID_TOKEN, result.Message);

        var Id = result.Claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Id is null)
        {
            return Result<User>.Failure(ERROR_CODE.INVALID_TOKEN, "No se ha encontrado el identificador del usuario");
        }

        var fetchResult = await _userRepository.FindFirstAsync(user => user.Id == Guid.Parse(Id));
        if (!fetchResult.IsSuccess) return Result<User>.Failure(fetchResult.Code, fetchResult.Message);
        var user = fetchResult.Data;
        if (!user.AccountVerified)
        {
            return Result<User>.Failure(ERROR_CODE.INVALID_AUTH_OPERATION,
                "Tu cuenta no ha sido verificada, verifica tu cuenta para poder restablecer tu contraseña");
        }

        if (user.PasswordToken != model.token)
        {
            return Result<User>.Failure(ERROR_CODE.INVALID_OPERATION,
                "Los datos no coinciden con los registrados, intentalo de nuevo");
        }

        var hashedPassword = new PasswordHasher<object>().HashPassword(null, model.password);
        user.Password = hashedPassword;
        user.PasswordToken = null;
        user.RestoreAttemptAt = null;
        user.RestoreAttemptCount = 0;
        var updateResult = await _userRepository.UpdateAsync(user);

        return !updateResult.IsSuccess
            ? Result<User>.Failure(updateResult.Code,
                "Ops! parece que hubo un error al registrar el usuario intentalo de nuevo")
            : Result<User>.Success("Contraseña restablecida correctamente");
    }


    public async Task<Result<User>> ChangePasswordAsync(ChangePwdDto model)
    {
        var result = await _userRepository.FindFirstAsync(user => user.Id == Guid.Parse(model.userId));
        if (!result.IsSuccess) return Result<User>.Failure(result.Code, result.Message);

        if (!result.Data.AccountVerified)
        {
            return Result<User>.Failure(ERROR_CODE.INVALID_AUTH_OPERATION,
                "Tu cuenta no ha sido verificada, verifica tu cuenta para poder cambiar tu contraseña");
        }

        var IsSamePassword =
            new PasswordHasher<object>().VerifyHashedPassword(null, result.Data.Password, model.password) ==
            PasswordVerificationResult.Success;
        if (!IsSamePassword)
        {
            return Result<User>.Failure(ERROR_CODE.INVALID_OPERATION,
                "La contraseña actual no coincide con la registrada");
        }

        var hashedPassword = new PasswordHasher<object>().HashPassword(null, model.newPassword);
        result.Data.Password = hashedPassword;
        var updateResult = await _userRepository.UpdateAsync(result.Data);
        return !updateResult.IsSuccess
            ? Result<User>.Failure(updateResult.Code, updateResult.Message)
            : Result<User>.Success("Contraseña actualizada correctamente");
    }


    public async Task<Result<User>> GetUserProfile(Guid id)
    {
        return await _userRepository.FindFirstAsync(user => user.Id == id);
    }

    public async Task<Result<IEnumerable<User>>> GetAllUsers()
    {
        return await _userRepository.GetAllAsync();
    }
}