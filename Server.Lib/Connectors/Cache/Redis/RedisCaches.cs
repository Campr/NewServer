using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.ScopeServices;
using StackExchange.Redis;

namespace Server.Lib.Connectors.Cache.Redis
{
    public class RedisCaches : Connector, ICaches
    {
        public RedisCaches(
            IJsonHelpers jsonHelpers,
            IConfiguration configuration)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));
            
            this.jsonHelpers = jsonHelpers;
            this.configuration = configuration;

            // Build the redis options from the provided configuration.
            this.redisOptions = new ConfigurationOptions
            {
                Password = configuration.RedisPassword,
                Ssl = true
            };

            // Copy the list of endpoints.
            foreach (var kv in configuration.RedisEndoints)
            {
                this.redisOptions.EndPoints.Add(kv.Key, kv.Value);
            }
             
            // Create the initializer for this component.
            this.initializer = new TaskRunner(this.InitializeOnceAsync);
        }
        
        private readonly IJsonHelpers jsonHelpers;
        private readonly IConfiguration configuration;
        
        private readonly TaskRunner initializer;
        private readonly ConfigurationOptions redisOptions;

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return this.initializer.RunOnce(cancellationToken);
        }

        private async Task InitializeOnceAsync(CancellationToken cancellationToken)
        {
            // Try to connect to the redis instance.
            var connection = await ConnectionMultiplexer.ConnectAsync(this.redisOptions);
            var database = connection.GetDatabase();
            
            // Create the resource store based on that connection.
            this.Users = new RedisCacheStore<CacheUser>(this.jsonHelpers, this.configuration, database);
        }

        public ICacheStore<CacheUser> Users { get; private set; }
    }
}