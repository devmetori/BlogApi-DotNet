namespace BlogApi.Shared.Models.Auth;

public class JwtToken
{
    public string Access { get; set; }
    public string Refresh { get; set; }
}