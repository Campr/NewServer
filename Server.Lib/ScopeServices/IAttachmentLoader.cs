using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IAttachmentLoader
    {
        /// <summary>
        /// Fetch a single <see cref="Attachment"/> by its Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Attachment"/> to fetch.</param>
        /// <param name="cancellationToken">Canceller.</param>
        /// <returns>The corresponding <see cref="Attachment"/> if found, <see langword="null"/> otherwise.</returns>
        Task<Attachment> FetchAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
    }
}