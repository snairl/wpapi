using API.DTOs;
using Application.DTOs;
using Application.Services.Categories;
using Application.Services.Users;
using Application.Services.WordPress;
using Domain.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

        private readonly ILogger<TestController> _logger;
        private readonly ICategoryService service;
        private readonly PostService postService;
        private readonly IUserService userService;

        public TestController(ILogger<TestController> logger,
            ICategoryService service,
            PostService postService,
            IUserService userService)
        {
            _logger = logger;
            this.service = service;
            this.postService = postService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct = default)
        {
            var categories = service.GetCategories();
            return Ok(categories);
        }

        [HttpGet]
        [Route("posts/{categoryId}/{page}")]
        public async Task<IActionResult> GetPost([FromRoute]string categoryId, [FromRoute]int page, CancellationToken ct = default)
        {
            var categories = service.GetPosts(categoryId, page);
            return Ok(categories);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login, CancellationToken ct = default)
        {
            var token = await userService.LoginAsync(login.Username, login.Password, ct);
            if(token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }

        [HttpGet("test")]
        [Authorize]
        public async Task<IActionResult> TestMe(CancellationToken ct = default)
        {
            return Ok("I AM OK");
        }
    }
}
