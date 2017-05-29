namespace Server.Lib.Connectors.Protocol
{
    public interface IProtocol
    {
        IProtocolClient MakeClient();
        IProtocolClient MakeAuthenticatedClient();
    }
}