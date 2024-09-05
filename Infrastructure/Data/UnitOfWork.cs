using Domain.Base;
using Domain.Categories;
using Domain.Interfaces;
using Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MongoDbContext _dbContext;

        public UnitOfWork(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IAsyncRepository<T> Repository<T>() where T : BaseEntity
        {
            return new RepositoryBase<T>(_dbContext.GetCollection<T>());
        }

    }
}
