namespace BlogApi.Shared.Models.Settings;

public class TwoFactorSettings
{
    public string SecretKey { get; set; }
    public int Expiration { get; set; }
}