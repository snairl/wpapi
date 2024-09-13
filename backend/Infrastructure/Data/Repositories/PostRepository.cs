using Domain.Categories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(IMongoCollection<Post> collection) : base(collection)
        {
        }
    }
}
