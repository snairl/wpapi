using Domain.Interfaces;
using Infrastructure.Data;
using MongoDB.Driver;
using System.Threading;

namespace Application.Services.MongoDb
{
    public class LockResource
    {
        public string Id { get; set; }
        public DateTime AcuqiredAt { get; set; }
        public DateTime ExpireAt { get; set; }
    }

    public class LockService : ILockService
    {
        private const int EXPIRE_PERIOD = 3000;
        private const int POLL_PERIOD = 300;
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

        public async Task LockAsync(string key)
        {
            var lockResource = new LockResource
            {
                Id = key,
                AcuqiredAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.Add(TimeSpan.FromMilliseconds(EXPIRE_PERIOD))
            };

            await _collection.InsertOneAsync(lockResource);
        }

        public async Task<bool> ReleaseLockAsync(string key)
        {
            var filter = Builders<LockResource>.Filter.Eq(doc => doc.Id, key);
            await _collection.DeleteOneAsync(filter);
            return true;
        }

        public async Task WaitForLockToBeReleased(string key)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var timeout = TimeSpan.FromMilliseconds(EXPIRE_PERIOD);
            var pollInterval = TimeSpan.FromMilliseconds(POLL_PERIOD);

            while (stopwatch.Elapsed < timeout)
            {
                bool isLocked = await IsLockedAsync(key);

                if (!isLocked)
                {
                    return;
                }

                await Task.Delay(pollInterval);
            }

            throw new TimeoutException($"Failed to acquire lock for {key} within the timeout period.");
        }
    }
}
