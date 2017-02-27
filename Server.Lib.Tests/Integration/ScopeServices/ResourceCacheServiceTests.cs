using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Server.Lib.Connectors.Caches;
using Server.Lib.Connectors.Tables;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.ScopeServices;
using Server.Lib.Tests.Infrastructure;
using Xunit;

namespace Server.Lib.Tests.Integration.ScopeServices
{
    [Collection("Global")]
    public class ResourceCacheServiceTests
    {
        public ResourceCacheServiceTests(GlobalFixture globalFixture)
        {
            this.global = globalFixture;
        }

        private readonly GlobalFixture global;

        #region Tests.

        [Fact]
        public async Task FailToFetchInexistingCacheDocument()
        {
            // Prepare.
            var expectedId = this.global.RandomId();
            var expectedCacheKey = $"id/{this.global.EncodeCacheKeyPart(expectedId)}";

            var userCacheMock = new Mock<ICacheStore<CacheUser>>();
            userCacheMock
                .Setup(u => u.GetAsync(expectedCacheKey, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Optional<CacheUser>(null)));

            var userTableMock = new Mock<IVersionedTable<CacheUser>>();
            var userLoader = this.CreateUserLoader(userCacheMock.Object, userTableMock.Object);

            // Act.
            var actualResource = await userLoader.FetchAsync(expectedId);

            // Assert.
            Assert.Null(actualResource);
            userCacheMock.Verify(u => u.GetAsync(expectedCacheKey, It.IsAny<CancellationToken>()), Times.Once);
            userTableMock.Verify(u => u.FindLastVersionAsync(It.IsAny<Expression<Func<CacheUser, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task FailToFetchInexistingTableDocument()
        {
            // Prepare.
            var expectedId = this.global.RandomId();
            var expectedCacheKey = $"id/{this.global.EncodeCacheKeyPart(expectedId)}";

            var userCacheMock = new Mock<ICacheStore<CacheUser>>();
            userCacheMock
                .Setup(u => u.GetAsync(expectedCacheKey, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Optional<CacheUser>()));

            var userTableMock = new Mock<IVersionedTable<CacheUser>>();
            userTableMock
                .Setup(u => u.FindLastVersionAsync(It.IsAny<Expression<Func<CacheUser, bool>>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((CacheUser)null));
            
            var userLoader = this.CreateUserLoader(userCacheMock.Object, userTableMock.Object);

            // Act.
            var actualResource = await userLoader.FetchAsync(expectedId);

            // Assert.
            Assert.Null(actualResource);
            userCacheMock.Verify(u => u.GetAsync(expectedCacheKey, It.IsAny<CancellationToken>()), Times.Once);
            userTableMock.Verify(u => u.FindLastVersionAsync(It.IsAny<Expression<Func<CacheUser, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FetchExistingCacheDocument()
        {
            // Prepare.
            var expectedUser = this.CreateExpectedCacheUser();
            var expectedCacheKey = $"id/{this.global.EncodeCacheKeyPart(expectedUser.Id)}";

            var userCacheMock = new Mock<ICacheStore<CacheUser>>();
            userCacheMock
                .Setup(u => u.GetAsync(expectedCacheKey, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new Optional<CacheUser>(expectedUser)));

            var userTableMock = new Mock<IVersionedTable<CacheUser>>();
            var userLoader = this.CreateUserLoader(userCacheMock.Object, userTableMock.Object);

            // Act.
            var actualResource = await userLoader.FetchAsync(expectedUser.Id);

            // Assert.
            Assert.NotNull(actualResource);
            AssertHelpers.HasEqualFieldValues(expectedUser, actualResource.ToCache());

            userCacheMock.Verify(u => u.GetAsync(expectedCacheKey, It.IsAny<CancellationToken>()), Times.Once);
            userTableMock.Verify(u => u.FindLastVersionAsync(It.IsAny<Expression<Func<CacheUser, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Boilerplate.

        private CacheUser CreateExpectedCacheUser()
        {
            return new CacheUser
            {
                Id = this.global.RandomId(),
                VersionId = this.global.RandomId(),
                Handle = "testhandle",
                Email = "testemail@domain.com",
                Entity = "https://entity.quentez.com",
                CreatedAt = DateTime.UtcNow
            };
        }

        private IUserLoader CreateUserLoader(ICacheStore<CacheUser> userCache, IVersionedTable<CacheUser> userTable)
        {
            var serviceProvider = this.CreateServiceProvider(userCache, userTable);
            return serviceProvider.GetService<IUserLoader>();
        }

        private IServiceProvider CreateServiceProvider(ICacheStore<CacheUser> userCache, IVersionedTable<CacheUser> userTable)
        {
            // Create the services collection.
            var services = new ServiceCollection();
            ServerLibInitializer.RegisterTypes(services, this.global.MakeTestConfiguration());

            // Mock the caches.
            var cachesMock = new Mock<ICaches>();
            cachesMock.SetupGet(c => c.Users).Returns(userCache);
            cachesMock.Setup(c => c.MakeForType<CacheUser>()).Returns(userCache);

            services.AddSingleton(cachesMock.Object);

            // Mock the database.
            var tablesMock = new Mock<ITables>();
            tablesMock.SetupGet(t => t.Users).Returns(userTable);

            services.AddSingleton(tablesMock.Object);
                
            // Create the service provider and return.
            return services.BuildServiceProvider(); 
        }

        #endregion
    }
}