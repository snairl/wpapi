using API.DTOs;
using API.Services;
using API.Services.Categories;
using API.Services.Users;
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
        private readonly WordPressPostService postService;
        private readonly IUserService userService;

        public TestController(ILogger<TestController> logger,
            ICategoryService service,
            WordPressPostService postService,
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
            var categories = await service.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("posts/{categoryId}/{page}")]
        public async Task<IActionResult> GetPost([FromRoute]string categoryId, [FromRoute]int page, CancellationToken ct = default)
        {
            var categories = await service.GetPostsAsync(categoryId, page, ct);
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
