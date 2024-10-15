using BlogApi.Api.Attributes;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using BlogApi.Shared.DTOs;

namespace BlogApi.Api.Controllers;

[Route("articles")]
[ApiController]
[IsAuthenticated]
public class ArticleController(IArticleService _articleService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> GetAllPublishedArticles()
    {
        var result = await _articleService.GetAllPublishedArticlesAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllArticles()
    {
        var result = await _articleService.GetAllArticlesAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("published/{id}")]
    public async Task<IActionResult> GetPublishedArticle([FromRoute] string id)
    {
        var result = await _articleService.GetPublishedArticleAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateArticle([FromBody] ArticleDto model)
    {
        var result = await _articleService.CreateArticleAsync(model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("authors")]
    public async Task<IActionResult> GetAllAuthors()
    {
        var result = await _articleService.GetAllAuthorsAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("author/{id}")]
    public async Task<IActionResult> GetAllArticlesByAuthor([FromRoute] string id)
    {
        var result = await _articleService.GetArticlesByAuthorAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPut("edit/{id}")]
    public async Task<IActionResult> EditArticle([FromRoute] string id, [FromBody] ArticleDto model)
    {
        var result = await _articleService.EditArticleAsync(Guid.Parse(id),model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteArticle([FromRoute] string id)
    {
        var result = await _articleService.DeleteArticleAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPatch("submit_for_review/{id}")]
    public async Task<IActionResult> SubmitArticleForReview([FromRoute] string id)
    {
        var result = await _articleService.SubmitArticleForReviewAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("all/review")]
    public async Task<IActionResult> GetAllArticlesForReview()
    {
        var result = await _articleService.GetAllArticlesForReviewAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("review/{id}")]
    public async Task<IActionResult> GetArticleForReview([FromRoute] string id)
    {
        var result = await _articleService.GetArticleForReviewAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPatch("review/{id}")]
    public async Task<IActionResult> ReviewArticle([FromRoute] string id, [FromBody] ReviewDto model)
    {
        var result = await _articleService.ReviewArticleAsync(Guid.Parse(id), model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("all/approval")]
    public async Task<IActionResult> GetAllArticlesForApproval()
    {
        var result = await _articleService.GetAllArticlesForApprovalAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("approval/{id}")]
    public async Task<IActionResult> GetArticleForApproval([FromRoute] string id)
    {
        var result = await _articleService.GetArticleForApprovalAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPatch("approval/{id}")]
    public async Task<IActionResult> ApprovalArticle([FromRoute] string id, [FromBody] ApprovalDto model)
    {
        var result = await _articleService.ApproveArticleAsync(Guid.Parse(id), model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPatch("publish/{id}")]
    public async Task<IActionResult> PublishArticle([FromRoute] string id)
    {
        var result = await _articleService.PublishArticleAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpPatch("unpublish/{id}")]
    public async Task<IActionResult> UnpublishArticle([FromRoute] string id)
    {
        var result = await _articleService.UnpublishArticleAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("audit/{id}")]
    public async Task<IActionResult> GetArticleAudit([FromRoute] string id)
    {
        var result = await _articleService.GetArticleAuditLogAsync( Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }

    [HttpGet("all/audit")]
    public async Task<IActionResult> GetAllArticlesAudit()
    {
        var result = await _articleService.GetAllAuditLogsAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { success = true, message = result.Message, data = result.Data });
    }
}