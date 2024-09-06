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
        //No filtering or sorting is needed for this query as it is a simple list of categories
        public IQueryable<CategoryDTO> GetCategories() 
            => categoryService.GetCategories();

        [Authorize]
        //No filtering or sorting is needed for this query as it is a simple list of posts
        public IQueryable<PostDTO> GetPosts(string categoryId, int page)
            => categoryService.GetPosts(categoryId, page);
    }
}
