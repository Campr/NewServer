using System;
using System.Threading;
using System.Threading.Tasks;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;
using Server.Lib.Models.Resources;
using Server.Lib.Services;

namespace Server.Lib.ScopeServices
{
    class UserLoader : IUserLoader
    {
        public UserLoader(
            IInternalUserLoader internalUserLoader,
            IExternalUserLoader externalUserLoader,
            ITaskHelpers taskHelpers,
            IUriHelpers uriHelpers)
        {
            Ensure.Argument.IsNotNull(internalUserLoader, nameof(internalUserLoader));
            Ensure.Argument.IsNotNull(externalUserLoader, nameof(externalUserLoader));
            Ensure.Argument.IsNotNull(taskHelpers, nameof(taskHelpers));
            Ensure.Argument.IsNotNull(uriHelpers, nameof(uriHelpers));

            this.internalUserLoader = internalUserLoader;
            this.externalUserLoader = externalUserLoader;
            this.taskHelpers = taskHelpers;
            this.uriHelpers = uriHelpers;
        }

        private readonly IInternalUserLoader internalUserLoader;
        private readonly IExternalUserLoader externalUserLoader;
        private readonly ITaskHelpers taskHelpers;
        private readonly IUriHelpers uriHelpers;
        
        public async Task<User> FetchByEntity(Uri entity, CancellationToken cancellationToken)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            // Try to retrieve the user internally.
            var user = await this.internalUserLoader.FetchByEntityAsync(entity, cancellationToken);
            if (user != null)
                return user;

            // If this is an internal entity, no need to look further.
            if (this.uriHelpers.TryGetHandleFromEntity(entity, out var handle))
                return null;

            // Retry logic to deal with race conditions on the user creation.
            return await this.taskHelpers.RetryAsync(async () =>
            {
                // Try to retrieve the user externally.
                user = await this.externalUserLoader.FetchByEntity(entity, cancellationToken);
                if (user == null)
                    return null;

                // If the user we found has a different entity, retry to fetch internally again.
                if (user.Entity != entity)
                {
                    var otherEntityUser = await this.internalUserLoader.FetchByEntityAsync(user.Entity, cancellationToken);
                    if (otherEntityUser != null)
                        return otherEntityUser;
                }

                // Otherwise, save it and return.
                await user.SaveAsync(cancellationToken);
                return user;
            }, cancellationToken);
        }
    }
}