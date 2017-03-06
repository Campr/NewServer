using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources.Posts;

namespace Server.Lib.Services
{
    public interface IPostLicenseLoader
    {
        /// <summary>
        /// Load a single <see cref="PostLicense"/> from the local collection.
        /// </summary>
        /// <param name="id">The Id of the <see cref="PostLicense"/> to load.</param>
        /// <param name="cancellationToken">Canceller.</param>
        /// <returns>The corresponding <see cref="PostLicense"/> if found, <see langword="null"/> otherwise.</returns>
        Task<PostLicense> LoadAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
    }
}