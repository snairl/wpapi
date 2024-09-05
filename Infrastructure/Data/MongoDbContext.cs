using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database = null;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            return _database.GetCollection<T>(typeof(T).Name);
        }

        public async Task CreateCollectionIfNotExists<T>()
        {
            var name = typeof(T).Name;
            var collectionList = (await _database.ListCollectionNamesAsync()).ToList();
            if (!collectionList.Contains(name))
            {
                await _database.CreateCollectionAsync(name);
                await SeedInitialDataAsync();
            }
        }

        private async Task SeedInitialDataAsync()
        {
            var collection = GetCollection<Domain.Users.User>();
            var user = new Domain.Users.User
            {
                Username = "admin",
            };
            user.SetPassword("nimda");
            await collection.InsertOneAsync(user);
        }
    }
}
