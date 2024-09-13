using Domain.Base;
using Domain.Categories;
using Domain.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;   

        public RepositoryBase(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public async Task<T> AddAsync(T entity, CancellationToken ct)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: ct);
            return entity;
        }

        public async Task<T> GetAsync(string id, CancellationToken ct)
        {
            return await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(ct);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression, CancellationToken ct)
        {
            return await _collection.Find(expression).FirstOrDefaultAsync(ct);
        }

        public IQueryable<T> ListAll(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Where(expression);
        }

        public async Task UpdateAsync(T entity, CancellationToken ct)
        {
            await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity, cancellationToken: ct);
        }

        public async Task DeleteAsync(string Id, CancellationToken ct)
        {
            await _collection.DeleteOneAsync(e => e.Id == Id, ct);
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities, CancellationToken ct)
        {
            entities.ForEach(async (e) => await AddAsync(e, ct));
            return await Task.FromResult(entities);
        }

        public async Task DeleteAllAsync(Expression<Func<T, bool>> expression, CancellationToken ct)
        {
            await _collection.DeleteManyAsync(expression, ct);
        }
    }
}
