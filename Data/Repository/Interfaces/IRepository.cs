using System.Linq.Expressions;
using BlogApi.Shared.Models;

namespace BlogApi.Data.Repository.Interfaces;
public interface IRepository<TEntity> where TEntity : class
{
    Task<Result<TEntity>> GetByIdAsync(string id);
    Task<Result<IEnumerable<TEntity>>> GetAllAsync();
    Task<Result<IEnumerable<TEntity>>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<Result<TEntity>> FindFirstAsync(Expression<Func<TEntity, bool>> predicate);
    
    Task<Result<TEntity>> AddAsync(TEntity entity);
    Task<Result<TEntity>> UpdateAsync(TEntity entity);
    Task<Result<TEntity>> DeleteAsync(TEntity entity);


   
}