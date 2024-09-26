using Microsoft.AspNetCore.Mvc;
using SourceFuse.Assessment.Common.Models;
using SourceFuse.Assessment.Common.Services;

namespace SourceFuse.Assessment.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginReqModel model)
        {
            _logger.LogInformation("Login attempt for user: {Username}", model.Username);

            var token = _authService.Login(model);
            if (token != null)
            {
                _logger.LogInformation("Login successful for user: {Username}", model.Username);
                return Ok(new { Token = token });
            }

            _logger.LogWarning("Login failed for user: {Username}", model.Username);
            return Unauthorized();
        }
    }
}