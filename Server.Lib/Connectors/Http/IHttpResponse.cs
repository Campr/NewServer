using System;
using System.Net;
using System.Threading.Tasks;

namespace Server.Lib.Connectors.Http
{
    public interface IHttpResponse
    {
        HttpStatusCode StatusCode { get; }
        bool IsSuccessStatusCode { get; }
        bool IsRetryableStatusCode { get; }

        Task<string> ReadContentAsStringAsync();
        Uri FindLinkInHeader(string linkRel);
    }

    public interface IHttpResponse<T> : IHttpResponse
    {
        Task<T> ReadContentAsync();
    }
}