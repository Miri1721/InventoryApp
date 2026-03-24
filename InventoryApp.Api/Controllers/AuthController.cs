using InventoryApp.Api.DTOs;
using InventoryApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Api.Controllers
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

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto request)
        {
            var result = _authService.Register(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var result = _authService.Login(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}