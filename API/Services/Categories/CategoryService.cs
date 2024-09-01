using API.DTOs;
using AutoMapper;
using Domain.Categories;
using Domain.Interfaces;
using System.Reflection.Metadata;

namespace API.Services.Categories
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly WordPressCategoryService wpCategoryService;
        private readonly WordPressPostService wpPostService;
        private readonly IMapper mapper;

        public CategoryService(IUnitOfWork unitOfWork,
            WordPressCategoryService wpCategoryService,
            WordPressPostService wpPostService,
            IMapper mapper) : base(unitOfWork)
        {
            this.wpCategoryService = wpCategoryService;
            this.wpPostService = wpPostService;
            this.mapper = mapper;
        }


        public async Task<List<CategoryDTO>> GetCategoriesAsync(CancellationToken ct = default)
        {        
            var repository = _unitOfWork.Repository<Category>();
            var categories = await repository.ListAllAsync(c => true, ct);

            if (categories.Count() == 0)
            {
                var wpCategories = await wpCategoryService.ListCategories(ct);
                categories = mapper.Map<List<Category>>(wpCategories);
                await repository.AddRangeAsync(categories, ct);
            }
            return mapper.Map<List<CategoryDTO>>(categories);
        }

        public async Task<List<PostDTO>> GetPostsAsync(string categoryId, int page, CancellationToken ct = default)
        {
            var categoryRepository = _unitOfWork.Repository<Category>();
            var category = await categoryRepository.GetAsync(categoryId, ct);
            var postRepository = _unitOfWork.Repository<Post>();

            if (category == null)
            {
                return new List<PostDTO>();
            }

            if(category.IsExpired)
            {
                var wpCategory = await wpCategoryService.GetCategory(category.WordPress_Id, ct);
                var categoryEntity = mapper.Map<Category>(wpCategory);
                if(!category.IsEqual(categoryEntity))
                {
                    category.Count = categoryEntity.Count;
                    await postRepository.DeleteAllAsync(p => p.CategoryId == category.Id, ct);
                }
                category.UpdateExpireTime();
                await categoryRepository.UpdateAsync(category, ct);
            }

            var posts = await postRepository.ListAllAsync(p => p.CategoryId == category.Id && p.Page == page, ct);
            if(posts.Count() == 0)
            {
                var wpPosts = await wpPostService.ListPosts(category.WordPress_Id, page, ct);
                posts = mapper.Map<List<Post>>(wpPosts);
                posts.ForEach(p => {
                    p.CategoryId = category.Id;
                    p.Page = page;
                });
                await postRepository.AddRangeAsync(posts, ct);
                //if (category.PostIds == null)
                //{
                //    category.PostIds = new List<string>();
                //}
                //category.PostIds.AddRange(posts.Select(p => p.Id));
                //await categoryRepository.UpdateAsync(category, ct);
            }
           

            return mapper.Map<List<PostDTO>>(posts);
        }
    }
}
