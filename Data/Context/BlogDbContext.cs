using BlogApi.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Data.Context;

public class BlogDbContext: DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options): base(options){ }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Session> Sessions { get; set; }
    
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Definir clave compuesta en RolePermission (relación M:N)
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Relación Role -> RolePermission
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.Authorizations)
            .HasForeignKey(rp => rp.RoleId);

        // Relación Permission -> RolePermission
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.Roles)
            .HasForeignKey(rp => rp.PermissionId);

        // Relación uno a muchos entre User y Role
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.SetNull); 

        // Relación uno a uno entre User y Session
        modelBuilder.Entity<Session>()
            .HasOne(s => s.User)
            .WithOne(u => u.Session)
            .HasForeignKey<Session>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación uno a muchos entre User y Article (autoría de artículos)
        modelBuilder.Entity<Article>()
            .HasOne(a => a.Author)
            .WithMany(u => u.Articles)
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade); 

        // Relación uno a muchos entre User y AuditLog
        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación uno a muchos entre Article y AuditLog (auditoría de artículos)
        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.Article)
            .WithMany(a => a.AuditLogs)
            .HasForeignKey(al => al.ResourceId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<User>().Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<User>().Property(u => u.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Session>().Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Session>().Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Role>().Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Role>().Property(r => r.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Permission>().Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Permission>().Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Article>().Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Article>().Property(a => a.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<AuditLog>().Property(al => al.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<AuditLog>().Property(al => al.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}