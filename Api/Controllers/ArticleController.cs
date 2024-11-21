using Microsoft.AspNetCore.Mvc;
using BlogApi.Api.Services.Interfaces;
using BlogApi.Shared.Exceptions;
using BlogApi.Shared.DTOs;
using BlogApi.Shared.Enums;
using System.Security.Claims;
using BlogApi.Api.Attributes;

namespace BlogApi.Api.Controllers;

[Route("articles")]
[IsAuthenticated]
public class ArticleController(IArticleService _articleService) : Controller
{

    [HttpGet]
    [IsAllowed([], PermissionName.READ_ARTICLE)]
    public async Task<IActionResult> GetAllPublishedArticles()
    {

        var result = await _articleService.GetAllPublishedArticlesAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }
    [HttpGet("{id}")]
    [IsAllowed([], PermissionName.READ_ARTICLE)]
    public async Task<IActionResult> GetArticle([FromRoute] Guid id)
    {
        var result = await _articleService.GetArticleAsync(id);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }


    [HttpGet("all")]
    [IsAllowed([], PermissionName.READ_ARTICLE)]
    public async Task<IActionResult> GetAllArticles()
    {
        var result = await _articleService.GetAllArticlesAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }


    [HttpPost("create")]
    [IsAllowed([RoleName.AUTHOR, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.CREATE_ARTICLE)]
    public async Task<IActionResult> CreateArticle([FromBody] ArticleDto article)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) HttpException.ThrowException(ERROR_CODE.UNAUTHORIZED, "Ha sorgido un error inesperado con tu sesión, por favor vuelve a intentarlo  o vuelve a iniciar sesión.");

        var result = await _articleService.CreateArticleAsync(Guid.Parse(userId), article);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpGet("authors")]
    [IsAllowed([], PermissionName.READ_ARTICLE)]
    public async Task<IActionResult> GetAllAuthors()
    {
        var result = await _articleService.GetAllAuthorsAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpGet("author/{id}")]
    [IsAllowed([], PermissionName.READ_ARTICLE)]
    public async Task<IActionResult> GetAllArticlesByAuthor([FromRoute] string id)
    {
        var result = await _articleService.GetArticlesByAuthorAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpPut("edit/{id}")]
    [IsAllowed([RoleName.AUTHOR, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.EDIT_ARTICLE)]
    [AuditAction]
    public async Task<IActionResult> EditArticle([FromRoute] string id, [FromBody] ArticleDto model)
    {
        var result = await _articleService.EditArticleAsync(Guid.Parse(id), model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpDelete("delete/{id}")]
    [IsAllowed([RoleName.AUTHOR, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.DELETE_ARTICLE)]
    public async Task<IActionResult> DeleteArticle([FromRoute] string id)
    {
        var result = await _articleService.DeleteArticleAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpPatch("submit_for_review/{id}")]
    [IsAllowed([RoleName.AUTHOR, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.EDIT_ARTICLE)]
    public async Task<IActionResult> SubmitArticleForReview([FromRoute] string id)
    {
        var result = await _articleService.SubmitArticleForReviewAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpGet("all/review")]
    [IsAllowed([RoleName.REVIEWER, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.READ_ARTICLE)]
    public async Task<IActionResult> GetAllArticlesForReview()
    {
        var result = await _articleService.GetAllArticlesForReviewAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }


    [HttpPatch("review/{id}")]
    [IsAllowed([RoleName.REVIEWER, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.REVIEW_ARTICLE)]

    public async Task<IActionResult> ReviewArticle([FromRoute] string id, [FromBody] ReviewDto model)
    {
        var result = await _articleService.ReviewArticleAsync(Guid.Parse(id), model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpGet("all/approval")]
    [IsAllowed([RoleName.EDITOR, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.READ_ARTICLE)]

    public async Task<IActionResult> GetAllArticlesForApproval()
    {
        var result = await _articleService.GetAllArticlesForApprovalAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }


    [HttpPatch("approval/{id}")]
    [IsAllowed([RoleName.EDITOR, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.APPROVE_ARTICLE)]

    public async Task<IActionResult> ApprovalArticle([FromRoute] string id, [FromBody] ApprovalDto model)
    {
        var result = await _articleService.ApproveArticleAsync(Guid.Parse(id), model);
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpPatch("publish/{id}")]
    [IsAllowed([RoleName.EDITOR, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.PUBLISH_ARTICLE)]

    public async Task<IActionResult> PublishArticle([FromRoute] string id)
    {
        var result = await _articleService.PublishArticleAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpPatch("unpublish/{id}")]
    [IsAllowed([RoleName.AUTHOR, RoleName.ADMIN, RoleName.SUPERUSUARIO], PermissionName.PUBLISH_ARTICLE)]
    public async Task<IActionResult> UnpublishArticle([FromRoute] string id)
    {
        var result = await _articleService.UnpublishArticleAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpGet("audit/{id}")]
    [IsAllowed([RoleName.SUPERUSUARIO], PermissionName.VIEW_AUDIT_LOGS)]

    public async Task<IActionResult> GetArticleAudit([FromRoute] string id)
    {
        var result = await _articleService.GetArticleAuditLogAsync(Guid.Parse(id));
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }

    [HttpGet("all/audit")]
    [IsAllowed([RoleName.SUPERUSUARIO], PermissionName.VIEW_AUDIT_LOGS)]
    public async Task<IActionResult> GetAllArticlesAudit()
    {
        var result = await _articleService.GetAllAuditLogsAsync();
        if (!result.IsSuccess) HttpException.ThrowException(result.Code, result.Message);
        return Ok(new { Success = true, Message = result.Message, Data = result.Data });
    }
}