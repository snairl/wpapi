using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("app")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public AppController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("{**url}")]
        public async Task<IActionResult> Get(string url)
        {
            var backendUrl = $"http://localhost:3000/{url}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, backendUrl);

            // Forward headers from the original request if needed
            foreach (var header in Request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, response.Content.Headers.ContentType?.ToString());
        }

    }
}
