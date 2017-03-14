namespace Server.Lib.Connectors.Http
{
    public interface IHttp
    {
        IHttpClient MakeClient();
        IHttpClient MakeAuthenticatedClient();
    }
}