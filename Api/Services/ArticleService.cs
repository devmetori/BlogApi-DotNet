
using BlogApi.Api.Services.Interfaces;
using BlogApi.Data.Entity;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.DTOs;
using BlogApi.Shared.Models;

namespace BlogApi.Api.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IAuditRepository _auditRepository;
    
    public ArticleService(IArticleRepository articleRepository, IAuditRepository auditRepository)
    {
        _articleRepository = articleRepository;
        _auditRepository = auditRepository;
    }


    public Task<Result<IEnumerable<Article>>> GetAllPublishedArticlesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> GetPublishedArticleAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<User>>> GetAllAuthorsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> GetArticlesByAuthorAsync(Guid authorId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> CreateArticleAsync(ArticleDto article)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> EditArticleAsync(Guid articleId, ArticleDto article)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> DeleteArticleAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> GetAllArticlesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> SubmitArticleForReviewAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> GetAllArticlesForReviewAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> GetArticleForReviewAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> ReviewArticleAsync(Guid id, ReviewDto review)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> GetAllArticlesForApprovalAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> GetArticleForApprovalAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> ApproveArticleAsync(Guid id, ApprovalDto approval)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> PublishArticleAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> UnpublishArticleAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuditLog>> GetArticleAuditLogAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<AuditLog>>> GetAllAuditLogsAsync()
    {
        throw new NotImplementedException();
    }
}