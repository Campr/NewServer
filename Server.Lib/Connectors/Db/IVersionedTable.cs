using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Db
{
    public interface IVersionedTable<TCacheResource> : ITable<TCacheResource> where TCacheResource : CacheVersionedResource
    {
        Task<TCacheResource> FindLastVersionAsync(Expression<Func<TCacheResource, bool>> filter, CancellationToken cancellationToken = default(CancellationToken));
    }
}