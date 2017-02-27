using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.ScopeServices
{
    public interface IUserLoader
    {
        User MakeNew();
        Task<User> FetchAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<User> FetchByEntityAsync(string entity, CancellationToken cancellationToken = default(CancellationToken));
        Task<User> FetchByEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveVersionAsync(User user, CancellationToken cancellationToken = default(CancellationToken));
    }
}