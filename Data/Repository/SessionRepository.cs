using BlogApi.Data.Repository.Interfaces;
using BlogApi.Data.Context;
using BlogApi.Data.Entity;


namespace BlogApi.Data.Repository;

public class SessionRepository(BlogDbContext context) : Repository<Session>(context), ISessionRepository;