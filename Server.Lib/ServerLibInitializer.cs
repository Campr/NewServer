using Microsoft.Extensions.DependencyInjection;
using Server.Lib.Connectors.Blobs;
using Server.Lib.Connectors.Blobs.Aws;
using Server.Lib.Connectors.Blobs.Azure;
using Server.Lib.Connectors.Blobs.File;
using Server.Lib.Connectors.Caches;
using Server.Lib.Connectors.Caches.Redis;
using Server.Lib.Connectors.Tables;
using Server.Lib.Connectors.Tables.Mongo;
using Server.Lib.Helpers;
using Server.Lib.ScopeServices;
using Server.Lib.Services;

namespace Server.Lib
{
    public static class ServerLibInitializer
    {
        public static void RegisterTypes(
            IServiceCollection services, 
            IConfiguration configuration)
        {
            // Register this configuration as a service.
            services.AddSingleton(configuration);

            // Base services.
            services.AddSingleton<ILoggingService, LoggingService>();

            // Helpers.
            services.AddSingleton<ITextHelpers, TextHelpers>();
            services.AddSingleton<IJsonHelpers, JsonHelpers>();
            services.AddSingleton<ITaskHelpers, TaskHelpers>();

            // Connectors.
            if (configuration.BlobsConnector == BlobsConnectors.Aws)
                services.AddSingleton<IBlobs, AwsBlobs>();
            else if (configuration.BlobsConnector == BlobsConnectors.Azure)
                services.AddSingleton<IBlobs, AzureBlobs>();
            else
                services.AddSingleton<IBlobs, FileBlobs>();

            // Caches is always Redis.
            services.AddSingleton<ICaches, RedisCaches>();

            // Tables is always Mongo.
            services.AddSingleton<ITables, MongoTables>();

            // Scoped services.
            services.AddScoped<IResourceCacheService, ResourceCacheService>();
            services.AddScoped<IInternalUserLoader, InternalUserLoader>();
        }
    }
}