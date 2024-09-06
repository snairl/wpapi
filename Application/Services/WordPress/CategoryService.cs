using Application.DTOs.WordPress;

namespace Application.Services.WordPress
{
    public class CategoryService(string siteUrl) : BaseService<CategoryDTO>(siteUrl)
    {
        private const string ENDPOINT = "wp-json/wp/v2/categories";

        public async Task<List<CategoryDTO>> ListCategories(CancellationToken ct)
        {
            return await GetAllAsync(ENDPOINT, new Dictionary<string, string>
            {
                { "per_page", "20" },
                { "page", "1" },
                { "orderby", "count" },
                { "order", "desc" },
                { "_fields", "id,name,description,count" },
            }, ct);
        }

        public async Task<CategoryDTO> GetCategory(string id, CancellationToken ct)
        {
            return await GetAsync($"{ENDPOINT}/{id}", new Dictionary<string, string>
            {
                { "_fields", "id,name,description,count" },
            }, ct);
        }
    }
}
