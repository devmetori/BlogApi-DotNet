using BlogApi.Shared.Enums;

namespace BlogApi.Data.Entity;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public  ICollection<RolePermission> Roles { get; set; }
}