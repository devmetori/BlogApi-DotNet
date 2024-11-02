using System.Linq.Expressions;
using BlogApi.Data.Entity;
using BlogApi.Shared.Models;

namespace BlogApi.Data.Repository.Interfaces;

public interface IUserRepository: IRepository<User>
{
    Task<Result<User>> FindUserWithRoleAsync(Expression<Func<User, bool>> predicate);
    Task<Result<User>> FindUserWithSessionAndRoleAsync(Expression<Func<User, bool>> predicate);
    
}