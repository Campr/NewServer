using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Db
{
    public interface ITable<TCacheResource> where TCacheResource : CacheResource
    {
        Task<TCacheResource> FindAsync(Expression<Func<TCacheResource, bool>> filter, CancellationToken cancellationToken = default(CancellationToken));
    }
}