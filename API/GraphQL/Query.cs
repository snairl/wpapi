using Application.DTOs;
using Application.Services.Categories;
using HotChocolate.Authorization;

namespace API.GraphQL
{
    public class Query
    {
        private readonly ICategoryService categoryService;

        public Query(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [Authorize]
        public async Task<IQueryable<CategoryDTO>> GetCategories() 
            => (await categoryService.GetCategoriesAsync()).AsQueryable();

        [Authorize]
        public async Task<IQueryable<PostDTO>> GetPosts(string categoryId, int page)
            => (await categoryService.GetPostsAsync(categoryId, page)).AsQueryable();
    }
}
