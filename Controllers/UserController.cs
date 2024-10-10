using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;

[Route("users")]
[ApiController]
public class UserController : Controller
{

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        return Ok(new { success = true, message = "Hello world" });
    }
    
    
    [HttpGet("{id}")]
    public IActionResult GetUserProfile( [FromRoute] string id)
    {
        return Ok(new { success = true, message = "Hello world" });
    }
  
}