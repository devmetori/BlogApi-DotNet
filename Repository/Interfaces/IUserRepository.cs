using BlogApi.Models;

namespace BlogApi.Repository.Interfaces;

public interface IUserRepository: IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
}