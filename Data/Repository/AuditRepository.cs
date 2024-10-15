

using BlogApi.Data.Repository.Interfaces;
using BlogApi.Data.Context;
using BlogApi.Data.Entity;
using BlogApi.Shared.Models;

namespace BlogApi.Data.Repository;

public class AuditRepository(BlogDbContext context) : Repository<AuditLog>(context), IAuditRepository
{
    public Task<Result<IEnumerable<AuditLog>>> GetAuditLogsByArticleId(Guid articleId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<AuditLog>>> GetAllAuditLogs()
    {
        throw new NotImplementedException();
    }
}