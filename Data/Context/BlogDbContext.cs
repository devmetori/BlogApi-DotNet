using BlogApi.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Context;

public class BlogDbContext(DbContextOptions<BlogDbContext> options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Session> Sessions { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Definir clave compuesta en RolePermission (relación M:N)
        builder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Relación Role -> RolePermission
        builder.Entity<RolePermission>().HasOne(rp => rp.Role).WithMany(r => r.Authorizations)
            .HasForeignKey(rp => rp.RoleId);

        // Relación Permission -> RolePermission
        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.Roles)
            .HasForeignKey(rp => rp.PermissionId);

        // Relación uno a muchos entre User y Role
        builder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relación uno a uno entre User y Session
        builder.Entity<Session>()
            .HasOne(s => s.User)
            .WithOne(u => u.Session)
            .HasForeignKey<Session>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación uno a muchos entre User y Article (autoría de artículos)
        builder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación uno a muchos entre User y AuditLog
        builder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación uno a muchos entre Article y AuditLog (auditoría de artículos)
        builder.Entity<AuditLog>()
            .HasOne(al => al.Article)
            .WithMany(a => a.AuditLogs)
            .HasForeignKey(al => al.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);

   
    }
}