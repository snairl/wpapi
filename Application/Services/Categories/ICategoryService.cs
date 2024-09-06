using Application.DTOs;

namespace Application.Services.Categories
{
    public interface ICategoryService
    {
        public IQueryable<CategoryDTO> GetCategories();
        public IQueryable<PostDTO> GetPosts(string categoryId, int page);
    }
}
