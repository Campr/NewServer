using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Models.Resources.Cache;

namespace Server.Lib.Connectors.Tables
{
    public interface IVersionedTable<TCacheResource> : ITable<TCacheResource> where TCacheResource : VersionedCacheResource
    {
        Task<TCacheResource> FindLastVersionAsync(Expression<Func<TCacheResource, bool>> filter, CancellationToken cancellationToken = default(CancellationToken));
    }
}