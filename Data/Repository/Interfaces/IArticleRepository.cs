using BlogApi.Shared.Models;
using BlogApi.Data.Entity;

namespace BlogApi.Data.Repository.Interfaces;

public interface IArticleRepository : IRepository<Article>
{
 
   
    Task<Result<IEnumerable<User>>> GetAllAuthorsAsync();
  
    
}