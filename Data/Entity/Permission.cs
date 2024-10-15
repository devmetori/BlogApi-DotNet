using BlogApi.Shared.Enums;

namespace BlogApi.Data.Entity;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public PermissionName Name { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<RolePermission> Roles { get; set; }
}