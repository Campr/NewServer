using System.Threading;
using System.Threading.Tasks;

namespace Server.Lib.Models.Resources.Factories
{
    public interface IUserFactory
    {
        User MakeNew();
        Task<User> FetchAsync(string userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<User> FetchByEntityAsync(string entity, CancellationToken cancellationToken = default(CancellationToken));
        Task<User> FetchByEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken));
    }
}