using API.DTOs;
using API.Services.Tokens;
using Application.DTOs;
using Application.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ITokenService tokenService;

        public LoginController(IUserService userService, ITokenService tokenService)
        {
            this.userService = userService;
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO login, CancellationToken ct = default)
        {
            var userDto = await userService.LoginAsync(login.Username, login.Password, ct);
            if (userDto == null)
            {
                return BadRequest("Invalid username or password");
            }
            var token = tokenService.GenerateToken(userDto.Username);
            return Ok(new LoggedDTO
            {
                Username = userDto.Username,
                Token = token
            });
        }
    }
}
