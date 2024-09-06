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

        public LoginController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO login, CancellationToken ct = default)
        {
            var token = await userService.LoginAsync(login.Username, login.Password, ct);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }
    }
}
