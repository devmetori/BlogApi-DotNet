using System;

namespace BlogApi.Shared.Models.Article;

public class AuditPayload
{


    public AuditPayload(string action, bool hasPermission, bool success, string message, Guid articleId, Guid userId)
    {
        Action = action;
        HasPermission = hasPermission;
        Success = success;
        Message = message;
        ArticleId = articleId;
        UserId = userId;
    }
    public string Action { get; set; }
    public bool HasPermission { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public Guid ArticleId { get; set; }
    public Guid UserId { get; set; }
}
