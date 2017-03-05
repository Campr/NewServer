using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;
using System.Linq;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Server.Lib.Connectors.Caches.Redis
{
    class RedisCaches : Connector, ICaches
    {
        public RedisCaches(
            IJsonHelpers jsonHelpers,
            IConfiguration configuration)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));

            this.jsonHelpers = jsonHelpers;
            this.configuration = configuration;

            // Create the initializer for this component.
            this.initializer = new TaskRunner(this.InitializeOnceAsync);
        }

        private readonly IJsonHelpers jsonHelpers;
        private readonly IConfiguration configuration;

        private readonly TaskRunner initializer;

        private IDatabase database;

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return this.initializer.RunOnce(cancellationToken);
        }

        private async Task InitializeOnceAsync(CancellationToken cancellationToken)
        {
            // Build the options for our Redis client.
            var redisOptions = new ConfigurationOptions
            {
                ResolveDns = true,
                Password = configuration.RedisPassword
            };

            // Resolve the IPs for all the Redis servers.
            var resolveServersTasks = this.configuration.RedisServers.Select(kv =>
                new KeyValuePair<Task<IPAddress[]>, int>(Dns.GetHostAddressesAsync(kv.Key), kv.Value)).ToList();

            await Task.WhenAll(resolveServersTasks.Select(kv => kv.Key));

            // Add them to the Redis options.
            var ipRedisServers = resolveServersTasks
                .Where(t => t.Key.Result != null)
                .SelectMany(kv => kv.Key.Result
                    .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                    .Select(ip => new KeyValuePair<IPAddress, int>(ip, kv.Value)))
                .ToList();
            foreach (var kv in ipRedisServers)
            {
                redisOptions.EndPoints.Add(kv.Key, kv.Value);
            }

            // Try to connect to the redis instance.
            var connection = await ConnectionMultiplexer.ConnectAsync(redisOptions);
            this.database = connection.GetDatabase();

            // Create the resource store based on that connection.
            this.Users = new RedisCacheStore<CacheUser>(this.jsonHelpers, this.configuration, this.database);
        }

        public ICacheStore<TCacheResource> MakeForType<TCacheResource>() where TCacheResource : CacheResource
        {
            return new RedisCacheStore<TCacheResource>(this.jsonHelpers, this.configuration, this.database);
        }

        public ICacheStore<CacheUser> Users { get; private set; }
    }
}