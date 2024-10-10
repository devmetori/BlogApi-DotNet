namespace BlogApi.Models;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Action { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Guid ResourceId { get; set; }
    public Guid UserId { get; set; }
    public bool HasPermissions { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }

    public User User { get; set; }
    public Article Article { get; set; }
}