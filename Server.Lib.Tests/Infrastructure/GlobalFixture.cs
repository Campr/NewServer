using System;
using System.Net;
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

        public string RandomId()
        {
            return Guid.NewGuid().ToString("N");
        }

        public string EncodeCacheKeyPart(string source)
        {
            return WebUtility.UrlEncode(source);
        }
    }

    [CollectionDefinition("Global")]
    public class GlobalCollection : ICollectionFixture<GlobalFixture>
    {
    }
}