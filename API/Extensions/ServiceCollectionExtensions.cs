using API.DTOs;
using API.GraphQL;
using API.Services;
using API.Services.Categories;
using API.Services.Users;
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

            services.AddScoped(serviceProvider =>
            {
                var client = serviceProvider.GetRequiredService<IMongoClient>();
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var database = client.GetDatabase(settings.DatabaseName);
                return database;
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

            services.AddScoped<WordPressCategoryService>(serviceProvider =>
            {
                return new WordPressCategoryService(siteUrl);
            });
            services.AddScoped<WordPressPostService>(serviceProvider =>
            {
                return new WordPressPostService(siteUrl);
            });
            services.Configure<JwtTokenSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICategoryService, CategoryService>();
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
                config.CreateMap<Category, DTOs.CategoryDTO>()
                    .ReverseMap();
                config.CreateMap<DTOs.WordPress.CategoryDTO, Category>()
                    .ForMember(d => d.Id, opt => opt.Ignore()) 
                    .ForMember(d => d.WordPress_Id, opt => opt.MapFrom(s => s.Id));

                config.CreateMap<Post, DTOs.PostDTO>()
                    .ReverseMap();
                config.CreateMap<DTOs.WordPress.PostDTO, Post>()
                    .ForMember(d => d.Id, opt => opt.Ignore())
                    .ForMember(d => d.WordPress_Id, opt => opt.MapFrom(s => s.Id));
            });

            services.AddScoped<IMapper>(serviceProvider =>
            {
                return mapperConfig.CreateMapper();
            });

            return services;
        }
    }
}
