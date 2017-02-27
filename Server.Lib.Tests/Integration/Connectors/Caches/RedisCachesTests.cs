using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Server.Lib.Connectors.Caches;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.Tests.Infrastructure;
using Xunit;

namespace Server.Lib.Tests.Integration.Connectors.Caches
{
    [Collection("Global")]
    public class RedisCachesTests
    {
        public RedisCachesTests(GlobalFixture globalFixture)
        {
            this.global = globalFixture;
        }

        private readonly GlobalFixture global;

        #region Tests.

        [Fact]
        public async Task FailToFetchInexistingDocument()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var expectedId = this.global.RandomId();

            // Act.
            var actualDocument = await redisCaches.Users.GetAsync(expectedId);

            // Assert.
            Assert.False(actualDocument.HasValue);
        }

        [Fact]
        public async Task StoreAndFetchNullValue()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var cacheKey = "null_value";

            // Act.
            await redisCaches.Users.SaveAsync(new [] { cacheKey }, null);
            var actualValue = await redisCaches.Users.GetAsync(cacheKey);

            // Assert.
            Assert.True(actualValue.HasValue);
            Assert.Null(actualValue.Value);
        }

        [Fact]
        public async Task FailToReplaceExistingWithNull()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var existingDocument = await this.CreateExpectedDocumentAsync(redisCaches);

            // Act.
            var saveTask = redisCaches.Users.SaveAsync(new [] { $"id/{this.global.EncodeCacheKeyPart(existingDocument.Id)}" }, null);

            // Assert.
            await Assert.ThrowsAsync<TaskCanceledException>(() => saveTask);
        }

        [Fact]
        public async Task FetchExistingById()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var expectedDocument = await this.CreateExpectedDocumentAsync(redisCaches);

            // Act.
            var actualDocument = await redisCaches.Users.GetAsync($"id/{this.global.EncodeCacheKeyPart(expectedDocument.Id)}");

            // Assert.
            AssertHelpers.HasEqualFieldValues(expectedDocument, actualDocument.Value);
        }

        [Fact]
        public async Task FetchExistingByOtherField()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var expectedDocument = await this.CreateExpectedDocumentAsync(redisCaches);

            // Act.
            var actualDocument = await redisCaches.Users.GetAsync($"entity/{this.global.EncodeCacheKeyPart(expectedDocument.Entity)}");

            // Assert.
            AssertHelpers.HasEqualFieldValues(expectedDocument, actualDocument.Value);
        }

        [Fact]
        public async Task ReplaceLastVersionWithNew()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var expectedDocumentVersion1 = await this.CreateExpectedDocumentAsync(redisCaches);
            var expectedDocumentVersion2 = await this.CreateExpectedDocumentAsync(redisCaches, expectedDocumentVersion1.Id, DateTime.UtcNow.AddHours(1));

            // Act.
            var actualDocument1 = await redisCaches.Users.GetAsync($"id/{this.global.EncodeCacheKeyPart(expectedDocumentVersion1.Id)}");
            var actualDocument2 = await redisCaches.Users.GetAsync($"id-version/{this.global.EncodeCacheKeyPart(expectedDocumentVersion1.Id)}/{this.global.EncodeCacheKeyPart(expectedDocumentVersion1.VersionId)}");
            var actualDocument3 = await redisCaches.Users.GetAsync($"id-version/{this.global.EncodeCacheKeyPart(expectedDocumentVersion2.Id)}/{this.global.EncodeCacheKeyPart(expectedDocumentVersion2.VersionId)}");

            // Assert.
            AssertHelpers.HasEqualFieldValues(expectedDocumentVersion2, actualDocument1.Value);
            AssertHelpers.HasEqualFieldValues(expectedDocumentVersion1, actualDocument2.Value);
            AssertHelpers.HasEqualFieldValues(expectedDocumentVersion2, actualDocument3.Value);
        }
         
        [Fact]
        public async Task DontReplaceLastVersionWithOld()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var expectedDocumentVersion1 = await this.CreateExpectedDocumentAsync(redisCaches);
            var expectedDocumentVersion2 = await this.CreateExpectedDocumentAsync(redisCaches, expectedDocumentVersion1.Id, DateTime.UtcNow.AddHours(-1));

            // Act.
            var actualDocument1 = await redisCaches.Users.GetAsync($"id/{this.global.EncodeCacheKeyPart(expectedDocumentVersion1.Id)}");
            var actualDocument2 = await redisCaches.Users.GetAsync($"id-version/{this.global.EncodeCacheKeyPart(expectedDocumentVersion1.Id)}/{this.global.EncodeCacheKeyPart(expectedDocumentVersion1.VersionId)}");
            var actualDocument3 = await redisCaches.Users.GetAsync($"id-version/{this.global.EncodeCacheKeyPart(expectedDocumentVersion2.Id)}/{this.global.EncodeCacheKeyPart(expectedDocumentVersion2.VersionId)}");

            // Assert.
            AssertHelpers.HasEqualFieldValues(expectedDocumentVersion1, actualDocument1.Value);
            AssertHelpers.HasEqualFieldValues(expectedDocumentVersion1, actualDocument2.Value);
            AssertHelpers.HasEqualFieldValues(expectedDocumentVersion2, actualDocument3.Value);
        }

        #endregion

        #region Boilerplate.

        private async Task<CacheUser> CreateExpectedDocumentAsync(ICaches caches, string id = null, DateTime? createdAt = null)
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

            // Create its various cache Ids.
            var cacheIds = new string[]
            {
                $"id/{this.global.EncodeCacheKeyPart(expectedDocument.Id)}",
                $"id-version/{this.global.EncodeCacheKeyPart(expectedDocument.Id)}/{this.global.EncodeCacheKeyPart(expectedDocument.VersionId)}",
                $"entity/{this.global.EncodeCacheKeyPart(expectedDocument.Entity)}",
                $"email/{this.global.EncodeCacheKeyPart(expectedDocument.Email)}"
            };

            // Add it to the cache.
            await caches.Users.SaveAsync(cacheIds, expectedDocument);
            return expectedDocument;
        }

        private async Task<ICaches> CreateRedisCachesAsync()
        {
            // Create the services collection, and initialize the connector.
            var services = new ServiceCollection();
            ServerLibInitializer.RegisterTypes(services, this.global.MakeTestConfiguration());
            var serviceProvider = services.BuildServiceProvider();

            // Create the class to test.
            var redisCaches = serviceProvider.GetService<ICaches>();

            // Initialize the caches.
            await redisCaches.InitializeAsync();
            return redisCaches;
        }
        
        #endregion
    }
}