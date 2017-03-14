using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IUserLoader
    {
        /// <summary>
        /// Fetch a <see cref="User"/>, internal or external, by its Entity.
        /// </summary>
        /// <param name="entity">The Entity of the <see cref="User"/> to fetch.</param>
        /// <param name="cancellationToken">Canceller.</param>
        /// <returns>The corresponding <see cref="User"/> if found, <see langword="null"/> otherwise.</returns>
        Task<User> FetchByEntity(Uri entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}