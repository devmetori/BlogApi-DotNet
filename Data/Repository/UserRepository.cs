

using BlogApi.Data.Context;
using BlogApi.Data.Entity;
using BlogApi.Data.Repository.Interfaces;

namespace BlogApi.Data.Repository;

public class UserRepository(BlogDbContext context) : Repository<User>(context), IUserRepository
{
    public Task<User> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}