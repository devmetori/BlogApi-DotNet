using BlogApi.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : Controller
    {

        [HttpPost("sign-up")]
        public IActionResult Signup([FromBody] SignUpDto model)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpPost("sign-in")]
        public IActionResult Signin([FromBody] SignInDto model)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpGet("verify-account/{id}")]
        public IActionResult VerifyAccount([FromRoute] string id)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpGet("verify-account/resend-email/{id}")]
        public IActionResult ResendVerifyAccountEmail([FromRoute] string id)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpPost("2fa/verify")]
        public IActionResult Verify2FA([FromBody] TwoFacAuthDto model)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpGet("2fa/resend/{id}")]
        public IActionResult Resend2FA([FromRoute] string id)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpGet("refresh-session")]
        public IActionResult RefreshSession()
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpDelete("sign-out")]
        public IActionResult Signout()
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpGet("forgot-password/{token}")]
        public IActionResult ValidateToken([FromRoute] string token)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpPatch("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto model)
        {
            return Ok(new { success = true, message = "Hello world" });
        }

        [HttpPatch("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePwdDto model)
        {
            return Ok(new { success = true, message = "Hello world" });
        }
    }
}