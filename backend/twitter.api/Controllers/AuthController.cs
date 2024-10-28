using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace twitter.api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        [Route("signup")]
        public async Task<IActionResult> signup()
        {
            return Ok("You hit the signup endpoint");
        }
    }
}
