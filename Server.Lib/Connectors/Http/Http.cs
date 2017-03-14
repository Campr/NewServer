using Server.Lib.Helpers;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Http
{
    class Http : IHttp
    {
        public Http(
            IJsonHelpers jsonHelpers,
            IHttpHelpers httpHelpers)
        {
            Ensure.Argument.IsNotNull(jsonHelpers, nameof(jsonHelpers));
            Ensure.Argument.IsNotNull(httpHelpers, nameof(httpHelpers));

            this.jsonHelpers = jsonHelpers;
            this.httpHelpers = httpHelpers;
        }

        private readonly IJsonHelpers jsonHelpers;
        private readonly IHttpHelpers httpHelpers;
        
        public IHttpClient MakeClient()
        {
            return new HttpClient(this.jsonHelpers, this.httpHelpers);
        }

        public IHttpClient MakeAuthenticatedClient()
        {
            throw new System.NotImplementedException();
        }
    }
}