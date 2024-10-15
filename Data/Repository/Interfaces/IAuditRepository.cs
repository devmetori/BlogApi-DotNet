using BlogApi.Shared.Models;
using BlogApi.Data.Entity;

namespace BlogApi.Data.Repository.Interfaces;

public interface IAuditRepository: IRepository<AuditLog>
{
    Task<Result<IEnumerable<AuditLog>>> GetAuditLogsByArticleId(Guid articleId);
    Task<Result<IEnumerable<AuditLog>>> GetAllAuditLogs();
    
}