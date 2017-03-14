using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Server.Lib.Connectors.Caches;
using Server.Lib.Models.Resources;
using Server.Lib.Models.Resources.Cache;
using Server.Lib.ScopeServices;
using Server.Lib.Tests.Infrastructure;
using Xunit;

namespace Server.Lib.Tests.Integration.ScopeServices
{
    [Collection("Global")]
    public class InternalUserLoaderTests
    {
        public InternalUserLoaderTests(GlobalFixture globalFixture)
        {
            this.global = globalFixture;
        }

        private readonly GlobalFixture global;

        #region Tests

        [Fact]
        public async Task FailToFetchInexistingResource()
        {
            // Prepare.
            var internalUserLoader = await this.CreateInternalUserLoader();
            var expectedId = this.global.RandomId();

            // Act.
            var actualResource = await internalUserLoader.FetchAsync(expectedId);

            // Assert.
            Assert.Null(actualResource);
        }

        [Fact]
        public async Task FetchExistingById()
        {
            // Prepare.
            var internalUserLoader = await this.CreateInternalUserLoader();
            var expectedUser = await this.CreateExpectedUserAsync(internalUserLoader);

            // Act.
            var actualUser = await internalUserLoader.FetchAsync(expectedUser.Id);

            // Assert.
            Assert.Equal(expectedUser, actualUser);
        }

        [Fact]
        public async Task FetchExistingByEmail()
        {
            // Prepare.
            var internalUserLoader = await this.CreateInternalUserLoader();
            var expectedUser = await this.CreateExpectedUserAsync(internalUserLoader);

            // Act.
            var actualUser = await internalUserLoader.FetchByEmailAsync(expectedUser.Email);

            // Assert.
            Assert.Equal(expectedUser, actualUser);
        }

        [Fact]
        public async Task FetchExistingByEntity()
        {
            // Prepare.
            var internalUserLoader = await this.CreateInternalUserLoader();
            var expectedUser = await this.CreateExpectedUserAsync(internalUserLoader);

            // Act.
            var actualUser = await internalUserLoader.FetchByEntityAsync(expectedUser.Entity);

            // Assert.
            Assert.Equal(expectedUser, actualUser);
        }

        #endregion

        #region Boilerplate.

        private async Task<User> CreateExpectedUserAsync(IInternalUserLoader internalUserLoader)
        {
            // Create the resource.
            var user = internalUserLoader.MakeNew(new CacheUser
            {
                Id = this.global.RandomId(),
                VersionId = this.global.RandomId(),
                Handle = "testhandle",
                Email = "testemail@domain.com",
                Entity = "https://entity.quentez.com",
                CreatedAt = DateTime.UtcNow
            });

            // Save it.
            await user.SaveAsync();
            return user;
        }

        private async Task<IInternalUserLoader> CreateInternalUserLoader()
        {
            var serviceProvider = await this.CreateServiceProvider();
            return serviceProvider.GetService<IInternalUserLoader>();
        }

        private async Task<IServiceProvider> CreateServiceProvider()
        {
            // Create the services collection, and initialize the connector.
            var services = new ServiceCollection();
            ServerLibInitializer.RegisterTypes(services, this.global.MakeTestConfiguration());
            var serviceProvider = services.BuildServiceProvider();

            // Initialize the caches.
            await serviceProvider.GetService<ICaches>().InitializeAsync();

            return serviceProvider;
        }

        #endregion
    }
}