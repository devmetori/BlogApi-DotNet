
using BlogApi.Api.Services.Interfaces;
using BlogApi.Data.Entity;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.DTOs;
using BlogApi.Shared.Enums;
using BlogApi.Shared.Models;
using BlogApi.Shared.Models.Article;

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


    public async Task<Result<IEnumerable<Article>>> GetAllPublishedArticlesAsync()
    {
        var result = await _articleRepository.FindAsync(x => x.Status == ArticleStatus.PUBLISHED.ToString());
        if (!result.IsSuccess) return result;
        return Result<IEnumerable<Article>>.Success(result.Data);
    }



    public async Task<Result<IEnumerable<User>>> GetAllAuthorsAsync()
    {
        var result = await _articleRepository.GetAllAuthorsAsync();
        if (!result.IsSuccess) return Result<IEnumerable<User>>.Failure(result.Code, result.Message);
        return Result<IEnumerable<User>>.Success(result.Data);
    }

    public async Task<Result<IEnumerable<Article>>> GetArticlesByAuthorAsync(Guid authorId)
    {
        var result = await _articleRepository.FindAsync(x => x.AuthorId == authorId);
        if (!result.IsSuccess) return result;
        return Result<IEnumerable<Article>>.Success(result.Data);
    }

    public async Task<Result<Article>> CreateArticleAsync(Guid AuthorId, ArticleDto article)
    {

        var result = await _articleRepository.AddAsync(new Article
        {
            Title = article.Title,
            Topic = article.Topic,
            Content = article.Content,
            Words = article.Content.Length,
            AuthorId = AuthorId,
            Status = ArticleStatus.DRAFT.ToString()
        });
        if (!result.IsSuccess) return Result<Article>.Failure(result.Code, result.Message);
        return Result<Article>.Success(result.Data);
    }

    public async Task<Result<Article>> EditArticleAsync(Guid articleId, ArticleDto article)
    {
        var result = await _articleRepository.FindFirstAsync(x => x.Id == articleId);
        if (!result.IsSuccess) return Result<Article>.Failure(result.Code, result.Message);

        result.Data.Title = article.Title;
        result.Data.Topic = article.Topic;
        result.Data.Content = article.Content;
        result.Data.Words = article.Content.Length;

        var updateAction = await _articleRepository.UpdateAsync(result.Data);
        if (!updateAction.IsSuccess) return Result<Article>.Failure(updateAction.Code, updateAction.Message);

        return Result<Article>.Success(result.Data);

    }

    public async Task<Result<Article>> DeleteArticleAsync(Guid id)
    {
        var result = await _articleRepository.FindFirstAsync(x => x.Id == id);
        if (!result.IsSuccess) return Result<Article>.Failure(result.Code,"No se encontro el articulo que desea eliminar");
        var deliteAction = await _articleRepository.DeleteAsync(result.Data);
        if (!deliteAction.IsSuccess) return Result<Article>.Failure(deliteAction.Code, deliteAction.Message);
        return Result<Article>.Success(result.Data);

    }

    public async Task<Result<IEnumerable<Article>>> GetAllArticlesAsync()
    {
        var result = await _articleRepository.GetAllAsync();
        if (!result.IsSuccess) return Result<IEnumerable<Article>>.Failure(result.Code, result.Message);
        return Result<IEnumerable<Article>>.Success(result.Data);
    }

    public async Task<Result<Article>> SubmitArticleForReviewAsync(Guid id)
    {
        var result = await _articleRepository.FindFirstAsync(x => x.Id == id);
        if (!result.IsSuccess) return Result<Article>.Failure(result.Code, result.Message);
        if (result.Data.Status != ArticleStatus.DRAFT.ToString()) return Result<Article>.Failure(ERROR_CODE.INVALID_OPERATION, "El articulo debe estar en estado borrador para poder enviarlo a revision");

        result.Data.Status = ArticleStatus.WAITING_FOR_REVIEW.ToString();
        var updateAction = await _articleRepository.UpdateAsync(result.Data);
        if (!updateAction.IsSuccess) return Result<Article>.Failure(updateAction.Code, updateAction.Message);

        return Result<Article>.Success(result.Data);
    }

    public async Task<Result<IEnumerable<Article>>> GetAllArticlesForReviewAsync()
    {
        var result = await _articleRepository.FindAsync(x => x.Status == ArticleStatus.WAITING_FOR_REVIEW.ToString());
        if (!result.IsSuccess) return result;
        return Result<IEnumerable<Article>>.Success(result.Data);
    }



    public async Task<Result<Article>> ReviewArticleAsync(Guid id, ReviewDto review)
    {
        var result = await _articleRepository.FindFirstAsync(x => x.Id == id);
        if (!result.IsSuccess) return Result<Article>.Failure(result.Code, result.Message);

        if (result.Data.Status != ArticleStatus.WAITING_FOR_REVIEW.ToString()) return Result<Article>.Failure(ERROR_CODE.INVALID_OPERATION, "El articulo debe estar en estado de revision para poder ser revisado");


        result.Data.Status = ArticleStatus.REVIEWED.ToString();
        result.Data.ReviewerComments = review.comment;

        var updateAction = await _articleRepository.UpdateAsync(result.Data);
        if (!updateAction.IsSuccess) return Result<Article>.Failure(updateAction.Code, updateAction.Message);
        return Result<Article>.Success(result.Data);

    }

    public async Task<Result<IEnumerable<Article>>> GetAllArticlesForApprovalAsync()
    {
        var result = await _articleRepository.FindAsync(x => x.Status == ArticleStatus.REVIEWED.ToString());
        if (!result.IsSuccess) return result;
        return Result<IEnumerable<Article>>.Success(result.Data);
    }

 

    public async Task<Result<Article>> ApproveArticleAsync(Guid id, ApprovalDto approval)
    {
        var result = await _articleRepository.FindFirstAsync(x => x.Id == id);
        if (!result.IsSuccess) return Result<Article>.Failure(result.Code, result.Message);
        var isAlreadyApproved = result.Data.Status == ArticleStatus.APPROVED.ToString();

        if (isAlreadyApproved) return Result<Article>.Failure(ERROR_CODE.INVALID_OPERATION, "El articulo ya ha sido aprobado");

        var isRejectedOrRequiredChanges = ArticleStatus.REJECTED_BY_EDITOR.ToString() == result.Data.Status || ArticleStatus.REQUIRES_CHANGES.ToString() == result.Data.Status;

        if (isRejectedOrRequiredChanges)
        {
            return Result<Article>.Failure(ERROR_CODE.INVALID_OPERATION, "El artículo ya sea rechado o sean enviador para ser modificada por el autor");
        }


        result.Data.Status = approval.Status.ToString();

        var updateAction = await _articleRepository.UpdateAsync(result.Data);
        if (!updateAction.IsSuccess) return Result<Article>.Failure(updateAction.Code, updateAction.Message);
        return Result<Article>.Success(result.Data);

    }

    public async Task<Result<Article>> PublishArticleAsync(Guid id)
    {
        var result = await _articleRepository.FindFirstAsync(x => x.Id == id);
        if (!result.IsSuccess) return Result<Article>.Failure(result.Code, result.Message);

        if (result.Data.Status != ArticleStatus.APPROVED.ToString()) return Result<Article>.Failure(ERROR_CODE.INVALID_OPERATION, "Es necesario que el articulo este aprobado para poder publicarlo");

        result.Data.Status = ArticleStatus.PUBLISHED.ToString();
        var updateAction = await _articleRepository.UpdateAsync(result.Data);
        if (!updateAction.IsSuccess) return Result<Article>.Failure(updateAction.Code, updateAction.Message);
        return Result<Article>.Success(result.Data);

    }

    public async Task<Result<Article>> UnpublishArticleAsync(Guid id)
    {
        var result = await _articleRepository.FindFirstAsync(x => x.Id == id);

        if (!result.IsSuccess) return Result<Article>.Failure(result.Code, result.Message);

        if (result.Data.Status != ArticleStatus.PUBLISHED.ToString()) return Result<Article>.Failure(ERROR_CODE.INVALID_OPERATION, "El articulo debe estar publicado para poder despublicarlo");

        result.Data.Status = ArticleStatus.ARCHIVED.ToString();

        var updateAction = await _articleRepository.UpdateAsync(result.Data);

        if (!updateAction.IsSuccess) return Result<Article>.Failure(updateAction.Code, updateAction.Message);


        return Result<Article>.Success(result.Data);

    }
    public async Task<Result<IEnumerable<AuditLog>>> GetArticleAuditLogAsync(Guid id)
    {
        var result = await _auditRepository.FindAsync(x => x.ArticleId == id);
        if (!result.IsSuccess) return Result<IEnumerable<AuditLog>>.Failure(result.Code, result.Message);
        return Result<IEnumerable<AuditLog>>.Success(result.Data);

    }

    public async Task<Result<IEnumerable<AuditLog>>> GetAllAuditLogsAsync()
    {
        var result = await _auditRepository.GetAllAsync();
        if (!result.IsSuccess) return Result<IEnumerable<AuditLog>>.Failure(result.Code, result.Message);
        return Result<IEnumerable<AuditLog>>.Success(result.Data);

    }

    public async Task<Result<AuditLog>> AuditActionAsync(AuditPayload payload)
    {


        var result = await _auditRepository.AddAsync(new AuditLog
        {
            Action = payload.Action,
            Success = payload.Success,
            Message = payload.Message,
            HasPermissions = payload.HasPermission,
            UserId = payload.UserId,
            ArticleId = payload.ArticleId

        });
        if (!result.IsSuccess) return Result<AuditLog>.Failure(result.Code, result.Message);
        return Result<AuditLog>.Success(result.Data);
    }

    public async Task<Result<Article>> GetArticleAsync(Guid id)
    {
        var result = await _articleRepository.FindFirstAsync(x => x.Id == id);
        if (!result.IsSuccess) return Result<Article>.Failure(result.Code, result.Message);
        return Result<Article>.Success(result.Data);

    }
}