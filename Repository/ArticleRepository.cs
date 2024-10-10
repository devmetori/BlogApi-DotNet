using BlogApi.Models;
using BlogApi.Repository.Interfaces;

namespace BlogApi.Repository;

public class ArticleRepository(BlogDbContext context) : Repository<Article>(context), IArticleRepository
{
    public Task<IEnumerable<Article>> GetArticlesByAuthorAsync(string authorId)
    {
        throw new NotImplementedException();
    }
}