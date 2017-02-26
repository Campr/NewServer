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
            var actualDocument = await redisCaches.Users.Get(expectedId);

            // Assert.
            Assert.False(actualDocument.HasValue);
        }

        [Fact]
        public async Task FetchExistingById()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var expectedDocument = await this.CreateExpectedDocumentAsync(redisCaches);

            // Act.
            var actualDocument = await redisCaches.Users.Get($"id/{this.global.EncodeCacheKeyPart(expectedDocument.Id)}");

            // Assert.
            Assert.NotNull(actualDocument);
            AssertHelpers.HasEqualFieldValues(expectedDocument, actualDocument.Value);
        }

        [Fact]
        public async Task FetchExistingByOtherField()
        {
            // Prepare.
            var redisCaches = await this.CreateRedisCachesAsync();
            var expectedDocument = await this.CreateExpectedDocumentAsync(redisCaches);

            // Act.
            var actualDocument = await redisCaches.Users.Get($"entity/{this.global.EncodeCacheKeyPart(expectedDocument.Entity)}");

            // Assert.
            Assert.NotNull(actualDocument);
            AssertHelpers.HasEqualFieldValues(expectedDocument, actualDocument.Value);
        }

        #endregion

        #region Boilerplate.

        private async Task<CacheUser> CreateExpectedDocumentAsync(ICaches caches)
        {
            // Create the document.
            var expectedDocument = new CacheUser
            {
                Id = this.global.RandomId(),
                VersionId = this.global.RandomId(),
                Handle = "testhandle",
                Email = "testemail@domain.com",
                Entity = "https://entity.quentez.com",
                CreatedAt = DateTime.UtcNow
            };

            // Create its variours cache Ids.
            var cacheIds = new string[]
            {
                $"id/{this.global.EncodeCacheKeyPart(expectedDocument.Id)}",
                $"id-version/{this.global.EncodeCacheKeyPart(expectedDocument.Id)}/{this.global.EncodeCacheKeyPart(expectedDocument.VersionId)}",
                $"entity/{this.global.EncodeCacheKeyPart(expectedDocument.Entity)}",
                $"email/{this.global.EncodeCacheKeyPart(expectedDocument.Email)}"
            };

            // Add it to the cache.
            await caches.Users.Save(cacheIds, expectedDocument);
            return expectedDocument;
        }

        private async Task<ICaches> CreateRedisCachesAsync()
        {
            // Mock the configuration that we'll provide to the connector.
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(c => c.RedisPassword).Returns((string)null);
            configurationMock.SetupGet(c => c.RedisServers).Returns(this.global.TestConfiguration.RedisServers);
            configurationMock.SetupGet(c => c.CachePrefixes).Returns(new Dictionary<Type, string>
            {
                { typeof(CacheUser), this.global.RandomId() }
            });

            // Create the services collection, and initialize the connector.
            var services = new ServiceCollection();
            ServerLibInitializer.RegisterTypes(services, configurationMock.Object);
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