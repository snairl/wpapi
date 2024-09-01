﻿using Domain.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Categories
{
    public class Category : BaseEntity
    {
        public DateTime ExpireTime { get; set; }
        public string WordPress_Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Count { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public List<String> PostIds { get; set; }

        public bool IsExpired => ExpireTime < DateTime.UtcNow;

        public Category()
        {
            PostIds = new List<string>();
            UpdateExpireTime();
        }
        
        public void UpdateExpireTime()
        {
            ExpireTime = DateTime.UtcNow.AddSeconds(5);
        }

        public bool IsEqual(Category other)
        {
            return Count != other.Count;
        }
    }
}
