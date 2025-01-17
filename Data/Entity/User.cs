﻿namespace BlogApi.Data.Entity;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? PasswordToken { get; set; }
    public int RestoreAttemptCount { get; set; } = 0;
    public DateTime? RestoreAttemptAt { get; set; }
    public bool AccountVerified { get; set; } = false;
    public bool Enabled2fa { get; set; } = true;
    public bool Verified2fa { get; set; } = false;
    public string? Secret2fa { get; set; }
    public string? Code2fa { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? RoleId { get; set; }
    public Role? Role { get; set; }
    public ICollection<Article> Articles { get; set; }
    public ICollection<AuditLog> AuditLogs { get; set; }
    public Session? Session { get; set; }
}