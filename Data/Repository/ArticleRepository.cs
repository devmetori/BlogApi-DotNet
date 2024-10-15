using BlogApi.Data.Context;
using BlogApi.Data.Entity;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.Enums;
using BlogApi.Shared.Models;
using BlogApi.Shared.Utils;

namespace BlogApi.Data.Repository;

public class ArticleRepository(BlogDbContext context) : Repository<Article>(context), IArticleRepository
{
    public async Task<Result<IEnumerable<Article>>> GetArticlesByAuthorAsync(Guid authorId)
    {
        try
        {
            var result = await this.FindAsync(m => m.AuthorId == authorId);

            if (!result.IsSuccess) return Result<IEnumerable<Article>>.Failure(result.Code, result.Message);

            if (result.Data == null || !result.Data.Any())
            {
                return Result<IEnumerable<Article>>.Failure(ERROR_CODE.RECORD_NOT_FOUND, "No sea encontrado ningún artículo para este autor");
            }

            return Result<IEnumerable<Article>>.Success(result.Data);
        }
        catch (Exception e)
        {
            return Result<IEnumerable<Article>>.Failure(ERROR_CODE.UNKNOWN_ERROR, e.Message);
        }
    }

    public async  Task<Result<IEnumerable<Article>>> EditArticle(Article newArticle)
    {
        try
        {
            var articleResult = await this.GetByIdAsync(newArticle.Id.ToString());
            
            if(!articleResult.IsSuccess) return Result<IEnumerable<Article>>.Failure(articleResult.Code, articleResult.Message);
            var article = articleResult.Data;
            article.Content = newArticle.Content;
            article.Title = newArticle.Title;
            article.Words = newArticle.Words;
            article.UpdatedAt = DateTime.UtcNow;
            var result = await this.UpdateAsync(article);
            if (!result.IsSuccess) return Result<IEnumerable<Article>>.Failure(result.Code, result.Message);
            return Result<IEnumerable<Article>>.Success();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<IEnumerable<Article>>.Failure(ERROR_CODE.UNKNOWN_ERROR, e.Message);
        }
    }

    public Task<Result<Article>> DeleteArticle(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> GetAllArticles()
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> GetPublishedArticles()
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> GetPublishedArticle(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> GetAllArticlesForReview()
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> GetArticleForReview(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Article>>> GetAllArticlesForApproval()
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> GetArticleForApproval(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> ApproveArticle(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> PublishArticle(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Article>> UnpublishArticle(Guid id)
    {
        throw new NotImplementedException();
    }
}