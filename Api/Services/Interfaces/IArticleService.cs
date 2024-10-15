using BlogApi.Data.Entity;
using BlogApi.Shared.DTOs;
using BlogApi.Shared.Models;

namespace BlogApi.Api.Services.Interfaces;

public interface IArticleService
{
    Task<Result<IEnumerable<Article>>> GetAllPublishedArticlesAsync();
    Task<Result<Article>> GetPublishedArticleAsync(Guid id);
    Task<Result<IEnumerable<User>>> GetAllAuthorsAsync();
    Task<Result<IEnumerable<Article>>> GetArticlesByAuthorAsync(Guid authorId);
    
    Task<Result<Article>> CreateArticleAsync(ArticleDto article);
    Task<Result<IEnumerable<Article>>> EditArticleAsync(Guid articleId, ArticleDto article);
    Task<Result<Article>> DeleteArticleAsync(Guid id);
    Task<Result<IEnumerable<Article>>> GetAllArticlesAsync();
    Task<Result<Article>> SubmitArticleForReviewAsync(Guid id);
    Task<Result<IEnumerable<Article>>> GetAllArticlesForReviewAsync();
    Task<Result<Article>> GetArticleForReviewAsync(Guid id);
    Task<Result<Article>> ReviewArticleAsync(Guid id, ReviewDto review);
    Task<Result<IEnumerable<Article>>> GetAllArticlesForApprovalAsync();
    Task<Result<Article>> GetArticleForApprovalAsync(Guid id);
    Task<Result<Article>> ApproveArticleAsync(Guid id, ApprovalDto approval );
    
    Task<Result<Article>> PublishArticleAsync(Guid id);
    Task<Result<Article>> UnpublishArticleAsync(Guid id);
    Task<Result<AuditLog>> GetArticleAuditLogAsync(Guid id);
    Task<Result<IEnumerable<AuditLog>>> GetAllAuditLogsAsync();
}