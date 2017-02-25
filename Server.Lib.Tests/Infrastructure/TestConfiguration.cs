using System;
using System.Collections.Generic;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Tests.Infrastructure
{
    public class TestConfiguration : IConfiguration
    {
        public TestConfiguration()
        {
            // Local mongo configuration.
            this.MongoShouldInitialize = true;
            this.MongoDebug = false;
            this.MongoDatabaseName = Guid.NewGuid().ToString("N");
            this.MongoServers = new []
            {
                new KeyValuePair<string, int>("localhost", 27017)
            };
            this.MongoCollections = new Dictionary<Type, string>
            {
                { typeof(CacheUser), "users" }
            };
            
            // Json.
            this.JsonDebug = false;
        }

        public string AwsAccessKey { get; }
        public string AwsAccessSecret { get; }
        public string AwsBlobsRegion { get; }
        public bool AzureBlobsShouldInitialize { get; }
        public string AzureBlobsConnectionString { get; }
        public string FileBlobsPath { get; }
        public IEnumerable<KeyValuePair<string, int>> RedisEndoints { get; }
        public string RedisPassword { get; }
        public IDictionary<Type, string> ResourceCacheKeys { get; }
        public bool MongoShouldInitialize { get; }
        public bool MongoDebug { get; }
        public IEnumerable<KeyValuePair<string, int>> MongoServers { get; }
        public IDictionary<Type, string> MongoCollections { get; }
        public string MongoDatabaseName { get; }
        public bool JsonDebug { get; }
    }
}