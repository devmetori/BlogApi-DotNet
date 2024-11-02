using BlogApi.Data.Entity;
using BlogApi.Shared.Models;

namespace BlogApi.Data.Repository.Interfaces;

public interface ISessionRepository: IRepository<Session>
{
    public Task<Result<Session>> CreateNewSession(Guid userId);
    
}