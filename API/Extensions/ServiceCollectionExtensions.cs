using Application.DTOs;
using API.GraphQL;
using Application.Services.Categories;
using Application.Services.Users;
using AutoMapper;
using Domain.Categories;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using API.Services.Tokens;
using Application.Services.WordPress;
using Domain.Users;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            services.AddScoped<MongoDbContext>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            //services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>))
            //    .AddScoped<ICategoryRepository, CategoryRepository>();
            return services;
        }
        
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, JwtTokenService>(serviceProvider => new JwtTokenService(serviceProvider.GetRequiredService<IOptions<JwtTokenSettings>>()));
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]))
                };
            });

            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            var siteUrl = configuration.GetValue<string>("WordPressSiteUrl") ?? default;

            if (string.IsNullOrEmpty(siteUrl))
            {
                throw new ArgumentNullException("WordPressSiteUrl is required");
            }

            services.AddScoped(serviceProvider =>
            {
                return new Application.Services.WordPress.CategoryService(siteUrl);
            });
            services.AddScoped<PostService>(serviceProvider =>
            {
                return new PostService(siteUrl);
            });
            services.Configure<JwtTokenSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICategoryService, Application.Services.Categories.CategoryService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }

        public static IServiceCollection ConfigureGraphQl(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<Query>()
                .AddType<CategoryDTO>();
            return services;
        }

        public static IServiceCollection ConfigureMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Category, Application.DTOs.CategoryDTO>()
                    .ReverseMap();
                config.CreateMap<Application.DTOs.WordPress.CategoryDTO, Category>()
                    .ForMember(d => d.Id, opt => opt.Ignore()) 
                    .ForMember(d => d.WordPress_Id, opt => opt.MapFrom(s => s.Id));

                config.CreateMap<Application.DTOs.WordPress.PostDTO, Post>()
                    .ForMember(d => d.Id, opt => opt.Ignore())
                    .ReverseMap();

                config.CreateMap<Application.DTOs.WordPress.PostDTO, Post>()
                    .ForMember(d => d.Id, opt => opt.Ignore())
                    .ForMember(d => d.WordPress_Id, opt => opt.MapFrom(s => s.Id));

                config.CreateMap<User, UserDTO>()
                    .ReverseMap();

                config.CreateMap<Post, Application.DTOs.PostDTO>();
            });

            services.AddScoped<IMapper>(serviceProvider =>
            {
                return mapperConfig.CreateMapper();
            });

            return services;
        }
    }
}
