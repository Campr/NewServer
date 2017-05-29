using System;
using System.Collections.Generic;
using Server.Lib.Connectors.Blobs;
using Server.Lib.Connectors.Queues;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Tests.Infrastructure
{
    public class TestConfiguration : IConfiguration
    {
        public TestConfiguration()
        {
            // Local Redis configuration.
            this.RedisServers = new []
            {
                new KeyValuePair<string, int>("localhost", 6379)
            };
            this.RedisPassword = null;
            this.CachePrefixes = new Dictionary<Type, string>
            {
                { typeof(CacheUser), Guid.NewGuid().ToString("N") }
            };

            // Local Mongo configuration.
            this.MongoShouldInitialize = true;
            this.MongoDebug = false;
            this.MongoDatabaseName = Guid.NewGuid().ToString("N");
            this.MongoServers = new []
            {
                new KeyValuePair<string, int>("localhost", 27017)
            };
            this.MongoCollections = new Dictionary<Type, string>
            {
                { typeof(CacheUser), Guid.NewGuid().ToString("N") }
            };

            // Json.
            this.JsonDebug = false;
        }

        public string AwsAccessKey { get; }
        public string AwsAccessSecret { get; }
        public BlobsConnectors BlobsConnector { get; }
        public string AwsBlobsRegion { get; }
        public bool AzureBlobsShouldInitialize { get; }
        public string AzureBlobsConnectionString { get; }
        public string FileBlobsPath { get; }
        public QueuesConnectors QueuesConnector { get; }
        public IEnumerable<KeyValuePair<string, int>> RedisServers { get; }
        public string RedisPassword { get; }
        public IDictionary<Type, string> CachePrefixes { get; }
        public bool MongoShouldInitialize { get; }
        public bool MongoDebug { get; }
        public IEnumerable<KeyValuePair<string, int>> MongoServers { get; }
        public IDictionary<Type, string> MongoCollections { get; }
        public string MongoDatabaseName { get; }
        public bool JsonDebug { get; }
    }
}