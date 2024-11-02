using System.Text;
using OtpNet;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Models.Settings;
using Microsoft.Extensions.Options;

namespace BlogApi.Api.Services;

public class TwoFactorAuthService(IOptions<TwoFactorSettings> twoFactorSettings) : ITwoFactorAuthService
{
    private TwoFactorSettings GetTwoFactorSettings()
    {
        var settings = twoFactorSettings.Value;
        if (settings is null) throw new NullReferenceException("TwoFactorSettings is null");
        return settings;
    }
    public (string, string) GenerateCode()
    {
       var settings = GetTwoFactorSettings();
        var secretKey = Encoding.UTF8.GetBytes(settings.SecretKey);
        var code  =  new Totp(secretKey: secretKey, step: settings.Expiration).ComputeTotp();
        return ( code, settings.SecretKey);
    }


    public bool IsValidCode(string secret, string code)
    {
        var settings = GetTwoFactorSettings();
        var secretKey = Encoding.UTF8.GetBytes(secret);
        var totp = new Totp(secretKey, step: settings.Expiration);
        return totp.VerifyTotp(code, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}