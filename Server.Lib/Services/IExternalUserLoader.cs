using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources;

namespace Server.Lib.Services
{
    public interface IExternalUserLoader
    {
        Task<User> FetchByEntity(Uri entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}