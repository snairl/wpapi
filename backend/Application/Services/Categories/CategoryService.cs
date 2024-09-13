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
        private readonly ILockService lockService;

        private readonly IAsyncRepository<Category> categoryRepository;
        private readonly IAsyncRepository<Post> postRepository;

        public CategoryService(IUnitOfWork unitOfWork,
            WordPress.CategoryService wpCategoryService,
            PostService wpPostService,
            IMapper mapper,
            ILockService lockService) : base(unitOfWork)
        {
            this.wpCategoryService = wpCategoryService;
            this.wpPostService = wpPostService;
            this.mapper = mapper;
            this.lockService = lockService;

            categoryRepository = unitOfWork.Repository<Category>();
            postRepository = unitOfWork.Repository<Post>();
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

        private async Task UpdateCategory(Category category)
        { 
            var wpCategory = await wpCategoryService.GetCategory(category.WordPress_Id, default);
            var categoryEntity = mapper.Map<Category>(wpCategory);
            if (!category.IsEqual(categoryEntity))
            {
                category.Count = categoryEntity.Count;
                await postRepository.DeleteAllAsync(p => p.CategoryId == category.Id, default);
            }
            category.UpdateExpireTime();
            await categoryRepository.UpdateAsync(category, default);
        }

        private async Task UpdatePosts(Category category, int page)
        {
            var wpPosts = wpPostService.ListPosts(category.WordPress_Id, page, default).Result;
            var postEntities = mapper.Map<List<Post>>(wpPosts);
            postEntities.ForEach(p =>
            {
                p.CategoryId = category.Id;
                p.Page = page;
            });
            await postRepository.AddRangeAsync(postEntities, default);
        }

        public IQueryable<PostDTO> GetPosts(string categoryId, int page)
        {
            var category = categoryRepository.GetAsync(categoryId, default).Result;
            if(category == null)
            {
                return Enumerable.Empty<PostDTO>().AsQueryable();
            }

            var categoryLockKey = category.Id;
            if (category.IsExpired && !lockService.IsLockedAsync(categoryLockKey).Result)
            {
                lockService.LockAsync(categoryLockKey).Wait();
                UpdateCategory(category).Wait();
                lockService.ReleaseLockAsync(categoryLockKey).Wait();
            }
            else
            {
                //Someone else is updating that category wait for it to finish
                lockService.WaitForLockToBeReleased(categoryLockKey).Wait();
            }

            var posts = postRepository.ListAll(p => p.CategoryId == categoryId && p.Page == page);
            var postLock = $"{categoryId}-{page}";
            if (posts.Count() == 0 && !lockService.IsLockedAsync(postLock).Result)
            {
                lockService.LockAsync(postLock).Wait();
                UpdatePosts(category, page).Wait();
                lockService.ReleaseLockAsync(postLock).Wait();
            }
            else
            {
                //Someone else is updating that category posts just wait for it to finish
                lockService.WaitForLockToBeReleased(postLock).Wait();
            }

            return posts.ProjectTo<PostDTO>(mapper.ConfigurationProvider);
        }
    }
}
