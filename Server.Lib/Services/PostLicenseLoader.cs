using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Extensions;
using Server.Lib.Models.Resources.Posts;

namespace Server.Lib.Services
{
    class PostLicenseLoader : IPostLicenseLoader
    {
        public PostLicenseLoader()
        {
            // This will need to be in database eventually.
            var licenseList = new List<PostLicense>
            {
                new PostLicense("cc-by-4.0", "Creative Commons Attribution 4.0", "https://creativecommons.org/licenses/by/4.0/")
            };

            this.licenses = new ReadOnlyDictionary<string, PostLicense>(licenseList.ToDictionary(l => l.Name, l => l));
        }

        private readonly IReadOnlyDictionary<string, PostLicense> licenses;

        public Task<PostLicense> LoadAsync(string id, CancellationToken cancellationToken = new CancellationToken())
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();

            if (!this.licenses.TryGetValue(id, out var license))
                return Task.FromResult((PostLicense)null);

            return Task.FromResult(license);
        }
    }
}