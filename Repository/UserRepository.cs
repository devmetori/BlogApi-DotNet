using BlogApi.Models;
using BlogApi.Repository.Interfaces;

namespace BlogApi.Repository;

public class UserRepository(BlogDbContext context) : Repository<User>(context), IUserRepository
{
    public Task<User> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}