using Domain.Interfaces;
using Infrastructure.Data;
using MongoDB.Driver;

namespace Application.Services.MongoDb
{
    public class LockResource
    {
        public string Id { get; set; }
        public DateTime ExpireAt { get; set; }
    }

    public class LockService : ILockService
    {
        private readonly IMongoCollection<LockResource> _collection;

        public LockService(MongoDbContext context)
        {
            _collection = context.GetCollection<LockResource>();
        }

        public async Task<bool> IsLockedAsync(string key)
        {
            var filter = Builders<LockResource>.Filter.Eq(res => res.Id, key);
            var lockResource = await _collection.Find(filter).FirstOrDefaultAsync();
            if(lockResource == null)
            {
                return false;
            }
            if(lockResource.ExpireAt< DateTime.UtcNow)
            {
                await ReleaseLockAsync(key);
                return false;
            }
            return true;
        }

        public async Task LockAsync(string key, TimeSpan expiry)
        {
            var lockResource = new LockResource
            {
                Id = key,
                ExpireAt = DateTime.UtcNow.Add(expiry)
            };

            await _collection.InsertOneAsync(lockResource);
        }

        public async Task<bool> ReleaseLockAsync(string key)
        {
            var filter = Builders<LockResource>.Filter.Eq(doc => doc.Id, key);
            await _collection.DeleteOneAsync(filter);
            return true;
        }
    }
}
