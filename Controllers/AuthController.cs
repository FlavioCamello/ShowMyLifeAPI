using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ShowMyLifeAPI.Models;
using ShowMyLifeAPI.Services.Interfaces;

namespace ShowMyLifeAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var user = await _userService.AuthenticateAsync(login.Email, login.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var token = user.GenerateJwtToken(user, _configuration["Jwt:Key"]);

            return Ok(new { token });
        }
    }
}
