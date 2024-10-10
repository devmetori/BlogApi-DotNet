using BlogApi.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

public class ArticleController : Controller
{
   [HttpGet]
    public IActionResult GetAllPublishedArticles()
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("all")]
    public IActionResult GetAllArticles()
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("published/{id}")]
    public IActionResult GetPublishedArticle([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpPost("create")]
    public IActionResult CreateArticle([FromBody] ArticleDto model)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("authors")]
    public IActionResult GetAllAuthors()
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("author/{id}")]
    public IActionResult GetAllArticlesByAuthor([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpPut("edit/{id}")]
    public IActionResult EditArticle([FromRoute] string id, [FromBody] ArticleDto model)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpDelete("delete/{id}")]
    public IActionResult DeleteArticle([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpPatch("submit_for_review/{id}")]
    public IActionResult SubmitForReview([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("all/review")]
    public IActionResult GetAllArticlesForReview()
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("review/{id}")]
    public IActionResult GetArticleForReview([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpPatch("review/{id}")]
    public IActionResult ReviewArticle([FromRoute] string id, [FromBody] ReviewDto model)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("all/approval")]
    public IActionResult GetAllArticlesForApproval()
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("approval/{id}")]
    public IActionResult GetArticleForApproval([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpPatch("approval/{id}")]
    public IActionResult ApprovalArticle([FromRoute] string id, [FromBody] ApprovalDto model)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpPatch("publish/{id}")]
    public IActionResult PublishArticle([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpPatch("unpublish/{id}")]
    public IActionResult UnpublishArticle([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("audit/{id}")]
    public IActionResult GetArticleAudit([FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }

    [HttpGet("all/audit")]
    public IActionResult GetAllArticlesAudit()
    {
        return Ok(new { success = true, message = "Hello world" });
    }
}