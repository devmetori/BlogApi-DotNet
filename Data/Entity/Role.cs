using BlogApi.Shared.Enums;

namespace BlogApi.Data.Entity;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = RoleName.READER.ToString();
    public RoleName Code { get; set; } = RoleName.READER;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public  ICollection<RolePermission> Authorizations { get; set; }
    public  ICollection<User> Users { get; set; }
}