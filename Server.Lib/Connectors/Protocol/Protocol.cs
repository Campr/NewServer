using Server.Lib.Connectors.Http;
using Server.Lib.Infrastructure;

namespace Server.Lib.Connectors.Protocol
{
    class Protocol : IProtocol
    {
        public Protocol(
            IConstants constants,
            IHttp http)
        {
            Ensure.Argument.IsNotNull(constants, nameof(constants));
            Ensure.Argument.IsNotNull(http, nameof(http));

            this.constants = constants;
            this.http = http;
        }

        private readonly IConstants constants;
        private readonly IHttp http;

        public IProtocolClient MakeClient()
        {
            return new ProtocolClient(this.constants, this.http.MakeClient());
        }

        public IProtocolClient MakeAuthenticatedClient()
        {
            return new ProtocolClient(this.constants, this.http.MakeAuthenticatedClient());
        }
    }
}