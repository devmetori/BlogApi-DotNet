using BlogApi.Data.Context;
using BlogApi.Data.Entity;
using BlogApi.Data.Repository.Interfaces;
using BlogApi.Shared.Enums;
using BlogApi.Shared.Models;
using BlogApi.Shared.Utils;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Result<IEnumerable<Article>>> EditArticle(Article newArticle)
    {
        try
        {
            var articleResult = await this.GetByIdAsync(newArticle.Id.ToString());

            if (!articleResult.IsSuccess) return Result<IEnumerable<Article>>.Failure(articleResult.Code, articleResult.Message);
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






    public async Task<Result<IEnumerable<User>>> GetAllAuthorsAsync()
    {

        try { 

            var result = await context.Articles.Select(m => m.Author).Distinct().ToListAsync();
            if(result == null || !result.Any()) return Result<IEnumerable<User>>.Failure(ERROR_CODE.RECORD_NOT_FOUND, "No se encontraron autores");
            return Result<IEnumerable<User>>.Success(result);
        } catch (Exception e) { 
            Console.WriteLine(e);
            return Result<IEnumerable<User>>.Failure(ERROR_CODE.UNKNOWN_ERROR, "Error al obtener autores");
        }


    }
}