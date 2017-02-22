using System;
using System.Collections.Generic;

namespace Server.Lib
{
    public interface IConfiguration
    {
        string AwsAccessKey { get; }
        string AwsAccessSecret { get; }
        string AwsBlobsRegion { get; }

        string AzureBlobsConnectionString { get; }

        string FileBlobsPath { get; }

        IEnumerable<KeyValuePair<string, int>> RedisEndoints { get; }
        string RedisPassword { get; }
        IDictionary<Type, string> ResourceCacheKeys { get; }

        IEnumerable<KeyValuePair<string, int>> MongoServers { get; }
        string MongoDatabaseName { get; }

        bool DebugJson { get; }
    }
}