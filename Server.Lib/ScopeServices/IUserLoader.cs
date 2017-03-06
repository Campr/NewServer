using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IUserLoader
    {
        User MakeNew();
        Task SaveVersionAsync(User user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Fetch the last version of a single <see cref="User"/> by its Id.
        /// </summary>
        /// <param name="userId">The Id of the <see cref="User"/> to fetch.</param>
        /// <param name="cancellationToken">Canceller.</param>
        /// <returns>The corresponding <see cref="User"/> if found, <see langword="null"/> otherwise.</returns>
        Task<User> FetchAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Fetch the last version of a single <see cref="User"/> by its Entity.
        /// </summary>
        /// <param name="entity">The Entity of the <see cref="User"/> to fetch.</param>
        /// <param name="cancellationToken">Canceller.</param>
        /// <returns>The corresponding <see cref="User"/> if found, <see langword="null"/> otherwise.</returns>
        Task<User> FetchByEntityAsync(string entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Fetch the last version of a single <see cref="User"/> by its Email.
        /// </summary>
        /// <param name="email">The email of the <see cref="User"/> to fetch.</param>
        /// <param name="cancellationToken">Canceller.</param>
        /// <returns>The corresponding <see cref="User"/> if found, <see langword="null"/> otherwise.</returns>
        Task<User> FetchByEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken));
    }
}