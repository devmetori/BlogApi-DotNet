using BlogApi.Models;
using BlogApi.Repository.Interfaces;

namespace BlogApi.Repository;

public class AuditRepository(BlogDbContext context) : Repository<AuditLog>(context), IAuditRepository;