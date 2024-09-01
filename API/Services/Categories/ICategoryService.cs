using API.DTOs;

namespace API.Services.Categories
{
    public interface ICategoryService
    {
        public Task<List<CategoryDTO>> GetCategoriesAsync(CancellationToken ct = default);
        public Task<List<PostDTO>> GetPostsAsync(string categoryId, int page, CancellationToken ct = default);
    }
}
