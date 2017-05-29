using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Server.Lib.Infrastructure;

namespace Server.Api.Middleware.Authentication
{
    public class HawkMiddleware : AuthenticationMiddleware<HawkOptions>
    {
        public HawkMiddleware(
            RequestDelegate next, 
            IOptions<HawkOptions> options, 
            ILoggerFactory loggerFactory, 
            UrlEncoder encoder,
            IServiceProvider serviceProvider) 
                : base(next, options, loggerFactory, encoder)
        {
            Ensure.Argument.IsNotNull(serviceProvider, nameof(serviceProvider));
            this.serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider serviceProvider;

        protected override AuthenticationHandler<HawkOptions> CreateHandler()
        {
            return this.serviceProvider.GetService<HawkHandler>();
        }
    }
}