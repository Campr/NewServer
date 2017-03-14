using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IInternalPostLoader
    {
        /// <summary>
        /// Fetch the last version of a single <see cref="Post"/>.
        /// </summary>
        /// <param name="user">The author of the <see cref="Post"/> to fetch.</param>
        /// <param name="postId">The Id of the <see cref="Post"/> to fetch.</param>
        /// <param name="cancellationToken">Canceller.</param>
        /// <returns>The corresponding <see cref="Post"/> if found, <see langword="null"/> otherwise.</returns>
        Task<Post> FetchAsync(User user, string postId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Fetch a specific version of a single <see cref="Post"/>.
        /// </summary>
        /// <param name="user">The author of the <see cref="Post"/> to fetch.</param>
        /// <param name="postId">The Id of the <see cref="Post"/> to fetch.</param>
        /// <param name="versionId">The Version Id of the <see cref="Post"/> to fetch.</param>
        /// <param name="cancellationToken">Canceller.</param>
        /// <returns>The corresponding <see cref="Post"/> if found, <see langword="null"/> otherwise.</returns>
        Task<Post> FetchAsync(User user, string postId, string versionId, CancellationToken cancellationToken = default(CancellationToken));
    }
}