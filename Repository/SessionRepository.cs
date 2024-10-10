using BlogApi.Models;
using BlogApi.Repository.Interfaces;

namespace BlogApi.Repository;

public class SessionRepository(BlogDbContext context) : Repository<Session>(context), ISessionRepository;