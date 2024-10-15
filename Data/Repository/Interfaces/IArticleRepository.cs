using BlogApi.Shared.Models;
using BlogApi.Data.Entity;

namespace BlogApi.Data.Repository.Interfaces;

public interface IArticleRepository : IRepository<Article>
{
    Task<Result<IEnumerable<Article>>> GetArticlesByAuthorAsync(Guid authorId);
    Task<Result<IEnumerable<Article>>> EditArticle(Article article);
    Task<Result<Article>> DeleteArticle(Guid id);
    Task<Result<IEnumerable<Article>>> GetAllArticles();
    Task<Result<IEnumerable<Article>>> GetPublishedArticles();
    Task<Result<Article>> GetPublishedArticle(Guid id);
    Task<Result<IEnumerable<Article>>> GetAllArticlesForReview();
    Task<Result<Article>> GetArticleForReview(Guid id);
    Task<Result<IEnumerable<Article>>> GetAllArticlesForApproval();
    Task<Result<Article>> GetArticleForApproval(Guid id);
    Task<Result<Article>> ApproveArticle(Guid id);
    
    Task<Result<Article>> PublishArticle(Guid id);
    Task<Result<Article>> UnpublishArticle(Guid id);
  
    
}