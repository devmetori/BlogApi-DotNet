namespace BlogApi.Shared.Models.Auth;

public class JwtPayload
{
    public string  Id { get; set;}
    public int Role { get; set;}
    public string SessionId { get; set;}
}