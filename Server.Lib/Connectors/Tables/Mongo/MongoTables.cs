using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.Services;

namespace Server.Lib.Connectors.Tables.Mongo
{
    class MongoTables : Connector, ITables
    {
        public MongoTables(
            IConfiguration configuration,
            ILoggingService loggingService)
        {
            Ensure.Argument.IsNotNull(configuration, nameof(configuration));
            Ensure.Argument.IsNotNull(loggingService, nameof(loggingService));

            this.configuration = configuration;

            // Configure the Mongo connection.
            var clientSettings = new MongoClientSettings
            {
                Servers = configuration.MongoServers.Select(kv => new MongoServerAddress(kv.Key, kv.Value)),
                ApplicationName = "MainApp",
                //WriteConcern = WriteConcern.WMajority,
                //ReadConcern = ReadConcern.Majority,
                ReadPreference = ReadPreference.Nearest,
                GuidRepresentation = GuidRepresentation.Standard
            };

            // If needed, set up logging.
            if (configuration.MongoDebug)
                clientSettings.ClusterConfigurator = cluster =>
                {
                    cluster.Subscribe<CommandStartedEvent>(e => loggingService.Info("[Mongo] {0} Started - {1}", e.CommandName, e.Command.ToJson()));
                    cluster.Subscribe<CommandSucceededEvent>(e => loggingService.Info("[Mongo] {0} Succeeded - {1}", e.CommandName, e.Reply.ToJson()));
                    cluster.Subscribe<CommandFailedEvent>(e => loggingService.Exception(e.Failure, "[Mongo] {0} Failed", e.CommandName));
                };

            // Configure serialization.
            var conventionPack = new ConventionPack();
            conventionPack.Add(new SnakeCaseElementNameConvention());
            ConventionRegistry.Register("snakeCase", conventionPack, t => true);

            // Create the client and get a reference to the Db.
            var client = new MongoClient(clientSettings);
            this.database = client.GetDatabase(configuration.MongoDatabaseName);

            // Create references to our tables.
            this.Users = new VersionedMongoTable<CacheUser>(this.database.GetCollection<CacheUser>(configuration.MongoCollections[typeof(CacheUser)]));

            // Create the initializer for this component.
            this.initializer = new TaskRunner(this.InitializeOnceAsync);
        }

        private readonly IConfiguration configuration;
        private readonly IMongoDatabase database;
        private readonly TaskRunner initializer;

        public override Task InitializeAsync(CancellationToken cancellationToken)
        {
            return this.initializer.RunOnce(cancellationToken);
        }

        private async Task InitializeOnceAsync(CancellationToken cancellationToken)
        {
            // Check if there's anything for us to do.  
            if (!this.configuration.MongoShouldInitialize)
                return;

            // List the collections in our database.
            var collectionsCursor = await this.database.ListCollectionsAsync(null, cancellationToken);
            var collections = await collectionsCursor.ToListAsync(cancellationToken);
            var collectionNames = collections.Select(c => c.GetValue("name").AsString).ToList();

            // If we don't have a "users" collection, create it now.
            var usersCollectionName = this.configuration.MongoCollections[typeof(CacheUser)];
            if (!collectionNames.Contains(usersCollectionName))
                await this.database.CreateCollectionAsync(usersCollectionName, null, cancellationToken);

            // List indexes for collection "users".
            var users = this.database.GetCollection<CacheUser>(usersCollectionName);
            var usersIndexesCursor = await users.Indexes.ListAsync(cancellationToken);
            var usersIndexes = await usersIndexesCursor.ToListAsync(cancellationToken);
            var usersIndexesNames = usersIndexes.Select(i => i.GetValue("name").AsString).ToList();

            // If the index for property "email" doesn't exist, create it now.
            if (!usersIndexesNames.Contains("email"))
                await users.Indexes.CreateOneAsync(
                    Builders<CacheUser>.IndexKeys.Ascending(u => u.Email),
                    new CreateIndexOptions
                    {
                        Name = "email",
                        Sparse = true,
                        Unique = true
                    },
                    cancellationToken);

            // If the index for property "entity" doesn't exist, create it now.
            if (!usersIndexesNames.Contains("entity"))
                await users.Indexes.CreateOneAsync(
                    Builders<CacheUser>.IndexKeys.Ascending(u => u.Entity),
                    new CreateIndexOptions
                    {
                        Name = "entity",
                        Unique = true
                    },
                    cancellationToken);
        }

        public IVersionedTable<CacheUser> Users { get; }
    }
}