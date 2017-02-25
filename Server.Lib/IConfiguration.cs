using System;
using System.Collections.Generic;

namespace Server.Lib
{
    public interface IConfiguration
    {
        string AwsAccessKey { get; }
        string AwsAccessSecret { get; }
        string AwsBlobsRegion { get; }

        bool AzureBlobsShouldInitialize { get; }
        string AzureBlobsConnectionString { get; }

        string FileBlobsPath { get; }

        IEnumerable<KeyValuePair<string, int>> RedisEndoints { get; }
        string RedisPassword { get; }
        IDictionary<Type, string> ResourceCacheKeys { get; }

        bool MongoShouldInitialize { get; }
        bool MongoDebug { get; }
        IEnumerable<KeyValuePair<string, int>> MongoServers { get; }
        IDictionary<Type, string> MongoCollections { get; }
        string MongoDatabaseName { get; }

        bool JsonDebug { get; }
    }
}