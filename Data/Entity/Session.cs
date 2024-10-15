﻿namespace BlogApi.Data.Entity;

public class Session
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime LastAccess { get; set; }
    public DateTime? ClosedAt { get; set; }
    public bool IsActive { get; set; } = false;
    public DateTime ExpiresAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}