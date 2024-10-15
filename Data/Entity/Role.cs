using BlogApi.Shared.Enums;

namespace BlogApi.Data.Entity;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public RoleName Name { get; set; } = RoleName.READER;
    public int Code { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<RolePermission> Authorizations { get; set; }
    public virtual ICollection<User> Users { get; set; }
}