using Microsoft.AspNetCore.Mvc;
using SourceFuse.Assessment.Common.Models;
using SourceFuse.Assessment.Common.Services;

namespace SourceFuse.Assessment.Api.Controllers
{
    namespace SourceFuse.Assessment.Api.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly IAuthService _authService;

            public AuthController(IAuthService authService)
            {
                _authService = authService;
            }

            [HttpPost("login")]
            public IActionResult Login([FromBody] LoginModel model)
            {
                var token = _authService.Login(model);
                if (token != null)
                {
                    return Ok(new { Token = token });
                }

                return Unauthorized();
            }
        }
    }
}
