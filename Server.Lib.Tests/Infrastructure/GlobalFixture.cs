using Xunit;

namespace Server.Lib.Tests.Infrastructure
{
    public class GlobalFixture
    {
        public GlobalFixture()
        {
            this.TestConfiguration = new TestConfiguration();
        }

        public IConfiguration TestConfiguration { get; }
    }

    [CollectionDefinition("Global")]
    public class GlobalCollection : ICollectionFixture<GlobalFixture>
    {
    }
}