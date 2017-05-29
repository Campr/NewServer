using Microsoft.AspNetCore.Builder;
using Server.Lib.Infrastructure;

namespace Server.Api.Middleware.Authentication
{
    public static class HawkAppBuilderExtensions
    {
        public static IApplicationBuilder UseHawkAuthentication(this IApplicationBuilder app, HawkOptions options)
        {
            Ensure.Argument.IsNotNull(app, nameof(app));
            Ensure.Argument.IsNotNull(options, nameof(options));

            return app.UseMiddleware<HawkMiddleware>(options);
        }
    }
}