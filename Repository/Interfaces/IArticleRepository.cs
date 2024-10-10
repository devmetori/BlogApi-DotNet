using BlogApi.Models;

namespace BlogApi.Repository.Interfaces;

public interface IArticleRepository: IRepository<Article>
{
    Task<IEnumerable<Article>> GetArticlesByAuthorAsync(string authorId);
}