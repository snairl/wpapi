using Application.DTOs;
using Application.Services.WordPress;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Categories;
using Domain.Interfaces;
using System.Reflection.Metadata;

namespace Application.Services.Categories
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly WordPress.CategoryService wpCategoryService;
        private readonly PostService wpPostService;
        private readonly IMapper mapper;

        public CategoryService(IUnitOfWork unitOfWork,
            WordPress.CategoryService wpCategoryService,
            PostService wpPostService,
            IMapper mapper) : base(unitOfWork)
        {
            this.wpCategoryService = wpCategoryService;
            this.wpPostService = wpPostService;
            this.mapper = mapper;
        }


        public IQueryable<CategoryDTO> GetCategories()
        {        
            var repository = _unitOfWork.Repository<Category>();
            var categories = repository.ListAll(c => true);

            if (categories.Count() == 0)
            {
                var wpCategories = wpCategoryService.ListCategories(default).Result;
                var categoryEntities = mapper.Map<List<Category>>(wpCategories);
                repository.AddRangeAsync(categoryEntities, default);
            }

            return categories.ProjectTo<CategoryDTO>(mapper.ConfigurationProvider);
        }

        public IQueryable<PostDTO> GetPosts(string categoryId, int page)
        {
            var categoryRepository = _unitOfWork.Repository<Category>();
            var category = categoryRepository.GetAsync(categoryId, default).Result;
            var postRepository = _unitOfWork.Repository<Post>();

            if (category.IsExpired)
            {
                var wpCategory = wpCategoryService.GetCategory(category.WordPress_Id, default).Result;
                var categoryEntity = mapper.Map<Category>(wpCategory);
                if (!category.IsEqual(categoryEntity))
                {
                    category.Count = categoryEntity.Count;
                    postRepository.DeleteAllAsync(p => p.CategoryId == category.Id, default);
                }
                category.UpdateExpireTime();
                categoryRepository.UpdateAsync(category, default);
            }

            var posts = postRepository.ListAll(p => p.CategoryId == category.Id && p.Page == page);
            if (posts.Count() == 0)
            {
                var wpPosts = wpPostService.ListPosts(category.WordPress_Id, page, default).Result;
                var postEntities = mapper.Map<List<Post>>(wpPosts);
                postEntities.ForEach(p =>
                {
                    p.CategoryId = category.Id;
                    p.Page = page;
                });
                postRepository.AddRangeAsync(postEntities, default);
                //if (category.PostIds == null)
                //{
                //    category.PostIds = new List<string>();
                //}
                //category.PostIds.AddRange(posts.Select(p => p.Id));
                //await categoryRepository.UpdateAsync(category, ct);
            }

            return posts.ProjectTo<PostDTO>(mapper.ConfigurationProvider);
        }
    }
}
