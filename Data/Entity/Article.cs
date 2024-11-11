using BlogApi.Shared.Enums;

namespace BlogApi.Data.Entity;

public class Article
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Topic { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int Words { get; set; }
    public string Status { get; set; } = ArticleStatus.DRAFT.ToString();
    public string? EditorComments { get; set; }
    public string? ReviewerComments { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid AuthorId { get; set; }
    public User Author { get; set; }
    public  ICollection<AuditLog> AuditLogs { get; set; }
}