using API.DTOs.WordPress;
using Newtonsoft.Json;

namespace API.Services
{
    public class WordPressPostService(string siteUrl) : BaseWordPressService<PostDTO>(siteUrl)
    {
        private const string ENDPOINT = "wp-json/wp/v2/posts";

        protected override List<PostDTO> deserializeJsonCollection(string content)
        {
            dynamic items = JsonConvert.DeserializeObject<List<dynamic>>(content);
            List<PostDTO> result = new();
            if (items != null)
            {
                foreach (var item in items)
                {
                    result.Add(new PostDTO
                    {
                        Id = item.id,
                        Title = item.title.rendered,
                        Slug = item.slug,
                        Date = item.date,
                        Link = item.link,
                    });
                }
            }
            return result;
        }

        public async Task<List<PostDTO>> ListPosts(string categoryId, int page, CancellationToken ct)
        {
            return await GetAllAsync(ENDPOINT, new Dictionary<string, string>
            {
                { "per_page", "10" },
                { "page", page.ToString() },
                { "orderby", "date" },
                { "order", "desc" },
                { "_fields", "id,title,slug,date,link" },
                { "status", "publish" },
                { "categories", categoryId },
            }, ct);
        }
    }
}
