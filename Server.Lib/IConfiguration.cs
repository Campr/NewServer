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

        IDictionary<string, int> RedisEndoints { get; }
        string RedisPassword { get; }
        IDictionary<Type, string> ResourceCacheKeys { get; }

        bool DebugJson { get; }
    }
}