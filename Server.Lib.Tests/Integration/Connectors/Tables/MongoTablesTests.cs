using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Server.Lib.Connectors.Tables;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.Tests.Infrastructure;
using Xunit;

namespace Server.Lib.Tests.Integration.Connectors.Tables
{
    [Collection("Global")]
    public class MongoTablesTests
    {
        public MongoTablesTests(GlobalFixture globalFixture)
        {
            this.global = globalFixture;
        }

        private readonly GlobalFixture global;

        #region Tests.

        [Fact]
        public async Task FailToFetchInexistingDocument()
        {
            // Prepare.
            var usersTable = this.CreateMongoUsersTable();
            var expectedId = this.global.RandomId();

            // Act.
            var actualDocument = await usersTable.FindAsync(u => u.Id == expectedId);

            // Assert.
            Assert.Null(actualDocument);
        }

        [Fact]
        public async Task FailToFetchInexistingLastVersion()
        {
            // Prepare.
            var usersTable = this.CreateMongoUsersTable();
            var expectedId = this.global.RandomId();

            // Act.
            var actualDocument = await usersTable.FindLastVersionAsync(u => u.Id == expectedId);

            // Assert.
            Assert.Null(actualDocument);
        }

        [Fact]
        public async Task FetchExistingById()
        {
            // Prepare.
            var usersTable = this.CreateMongoUsersTable();
            var expectedDocument = await this.CreateExpectedDocumentAsync(usersTable);

            // Act.
            var actualDocument = await usersTable.FindAsync(u => u.Id == expectedDocument.Id);

            // Assert.
            AssertHelpers.HasEqualFieldValues(expectedDocument, actualDocument);
        }

        [Fact]
        public async Task FetchExistingByOtherField()
        {
            // Prepare.
            var usersTable = this.CreateMongoUsersTable();
            var expectedDocument = await this.CreateExpectedDocumentAsync(usersTable);

            // Act.
            var actualDocument = await usersTable.FindAsync(u => u.Email == expectedDocument.Email);

            // Assert.
            AssertHelpers.HasEqualFieldValues(expectedDocument, actualDocument);
        }

        [Fact]
        public async Task FailToInsertTwice()
        {
            // Prepare.
            var usersTable = this.CreateMongoUsersTable();
            var document = await this.CreateExpectedDocumentAsync(usersTable);

            // Act.
            var insertTask = usersTable.InsertAsync(document);

            // Assert.
            await Assert.ThrowsAsync<MongoWriteException>(() => insertTask);
        }

        [Fact]
        public async Task FetchLastVersion()
        {
            // Prepare.
            var usersTable = this.CreateMongoUsersTable();
            var expectedDocumentVersion1 = await this.CreateExpectedDocumentAsync(usersTable);
            var expectedDocumentVersion2 = await this.CreateExpectedDocumentAsync(usersTable, expectedDocumentVersion1.Id, DateTime.UtcNow.AddHours(1));

            // Act.
            var actualDocument = await usersTable.FindLastVersionAsync(u => u.Id == expectedDocumentVersion1.Id);
            
            // Assert.
            AssertHelpers.HasEqualFieldValues(expectedDocumentVersion2, actualDocument);
        }

        #endregion

        #region Boilerplate.

        private async Task<CacheUser> CreateExpectedDocumentAsync(IVersionedTable<CacheUser> usersTable, string id = null, DateTime? createdAt = null)
        {
            // Create the document.
            var expectedDocument = new CacheUser
            {
                Id = id ?? this.global.RandomId(),
                VersionId = this.global.RandomId(),
                Handle = "testhandle",
                Email = "testemail@domain.com",
                Entity = "https://entity.quentez.com",
                CreatedAt = createdAt.GetValueOrDefault(DateTime.UtcNow)
            };

            // Add it to the collection.
            await usersTable.InsertAsync(expectedDocument);
            return expectedDocument;
        }

        private IVersionedTable<CacheUser> CreateMongoUsersTable()
        {
            // Create the services collection, and initialize the connector.
            var services = new ServiceCollection();
            ServerLibInitializer.RegisterTypes(services, this.global.MakeTestConfiguration());
            var serviceProvider = services.BuildServiceProvider();

            // Create the class to test.
            var mongoTables = serviceProvider.GetService<ITables>();

            // Initialize the tables.
            //await mongoTables.InitializeAsync(new CancellationToken());
            return mongoTables.TableForVersionedType<CacheUser>();
        }

        #endregion
    }
}