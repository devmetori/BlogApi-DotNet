using BlogApi.Data.Entity;

namespace BlogApi.Data.Repository.Interfaces;

public interface IUserRepository: IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    
}