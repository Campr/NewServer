using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Moq;
using Server.Lib.Connectors.Db.Mongo;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.Services;
using Server.Lib.Tests.Infrastructure;
using Xunit;

namespace Server.Lib.Tests.Integration.Connectors.Db
{
    [Collection("Global")]
    public class MongoTablesTests
    {
        public MongoTablesTests(GlobalFixture globalFixture)
        {
            this.testConfiguration = globalFixture.TestConfiguration;
        }

        private readonly IConfiguration testConfiguration;

        [Fact]
        public async Task FailToFetchInexistingDocument()
        {
            // Prepare.
            var mongoTables = this.CreateMongoTables();
            var expectedId = Guid.NewGuid().ToString("N");

            // Act.
            var actualDocument = await mongoTables.Users.FindAsync(u => u.Id == expectedId);

            // Assert.
            Assert.Null(actualDocument);
        }

        [Fact]
        public async Task FetchExistingById()
        {
            // Prepare.
            var mongoTables = this.CreateMongoTables();
            var expectedDocument = await this.CreateExpectedDocumentAsync(mongoTables);

            // Act.
            var actualDocument = await mongoTables.Users.FindAsync(u => u.Id == expectedDocument.Id);

            // Assert.
            Assert.NotNull(actualDocument);
            Assert.Equal(expectedDocument.Handle, actualDocument.Handle);
        }

        [Fact]
        public async Task FetchExistingByOtherField()
        {
            // Prepare.
            var mongoTables = this.CreateMongoTables();
            var expectedDocument = await this.CreateExpectedDocumentAsync(mongoTables);

            // Act.
            var actualDocument = await mongoTables.Users.FindAsync(u => u.Email == expectedDocument.Email);

            // Assert.
            Assert.NotNull(actualDocument);
            Assert.Equal(expectedDocument.Handle, actualDocument.Handle);
        }

        [Fact]
        public async Task FailToInsertTwice()
        {
            // Prepare.
            var mongoTables = this.CreateMongoTables();
            var document = await this.CreateExpectedDocumentAsync(mongoTables);

            // Act + Assert.
            await Assert.ThrowsAsync<MongoWriteException>(() => mongoTables.Users.InsertAsync(document));
        }

        [Fact]
        public async Task FetchLastVersion()
        {
            // Prepare.
            var mongoTables = this.CreateMongoTables();
            var expectedDocumentVersion1 = await this.CreateExpectedDocumentAsync(mongoTables);
            var expectedDocumentVersion2 = await this.CreateExpectedDocumentAsync(mongoTables, expectedDocumentVersion1.Id, DateTime.UtcNow.AddHours(1));

            // Act.
            var actualDocument = await mongoTables.Users.FindLastVersionAsync(u => u.Id == expectedDocumentVersion1.Id);
            
            // Assert.
            Assert.NotNull(actualDocument);
            Assert.Equal(expectedDocumentVersion2.VersionId, actualDocument.VersionId);
        }

        private async Task<CacheUser> CreateExpectedDocumentAsync(MongoTables mongoTables, string id = null, DateTime? createdAt = null)
        {
            // Create the document.
            var expectedDocument = new CacheUser
            {
                Id = id ?? Guid.NewGuid().ToString("N"),
                VersionId = Guid.NewGuid().ToString("N"),
                Handle = "testhandle",
                Email = "testemail@domain.com",
                Entity = "https://entity.quentez.com",
                CreatedAt = createdAt.GetValueOrDefault(DateTime.UtcNow)
            };

            // Add it to the collection.
            await mongoTables.Users.InsertAsync(expectedDocument);
            return expectedDocument;
        }

        private MongoTables CreateMongoTables()
        {
            // Mock the configuration that we'll provide to the connector.
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(c => c.MongoShouldInitialize).Returns(true);
            configurationMock.SetupGet(c => c.MongoDebug).Returns(false);
            configurationMock.SetupGet(c => c.MongoServers).Returns(this.testConfiguration.MongoServers);
            configurationMock.SetupGet(c => c.MongoDatabaseName).Returns(this.testConfiguration.MongoDatabaseName);
            configurationMock.SetupGet(c => c.MongoCollections).Returns(new Dictionary<Type, string>
            {
                { typeof(CacheUser), Guid.NewGuid().ToString("N") }
            });

            // Mock the logger.
            var loggingServiceMock = new Mock<ILoggingService>();

            // Create the class to test.
            var mongoTables = new MongoTables(configurationMock.Object, loggingServiceMock.Object);

            // Initialize the tables.
            //await mongoTables.InitializeAsync(new CancellationToken());
            return mongoTables;
        }
    }
}