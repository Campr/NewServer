using System;
using System.Collections.Generic;
using Server.Lib.Connectors.Blobs;
using Server.Lib.Connectors.Queues;

namespace Server.Lib
{
    public interface IConfiguration
    {
        // General AWS configuration.
        string AwsAccessKey { get; }
        string AwsAccessSecret { get; }

        // Blobs connector.
        BlobsConnectors BlobsConnector { get; }

        string AwsBlobsRegion { get; }

        bool AzureBlobsShouldInitialize { get; }
        string AzureBlobsConnectionString { get; }

        string FileBlobsPath { get; }

        // Queues connector.
        QueuesConnectors QueuesConnector { get; }

        // Caches connector.
        IEnumerable<KeyValuePair<string, int>> RedisServers { get; }
        string RedisPassword { get; }
        IDictionary<Type, string> CachePrefixes { get; }

        // Db connector.
        bool MongoShouldInitialize { get; }
        bool MongoDebug { get; }
        IEnumerable<KeyValuePair<string, int>> MongoServers { get; }
        IDictionary<Type, string> MongoCollections { get; }
        string MongoDatabaseName { get; }

        // Json.
        bool JsonDebug { get; }
    }
}