using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Server.Lib.Models.Resources.Cache
{
    public abstract class CacheResource
    {
        [BsonId]
        [JsonIgnore]
        public virtual string DbId => this.Id;

        [BsonElement("id")]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}