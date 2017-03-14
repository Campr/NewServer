using System;
using System.Net;

namespace Server.Lib.Connectors.Http
{
    public class HttpException : Exception
    {
        public HttpException(HttpStatusCode statusCode, string message = null)
        {
            // TODO: Do something with those values.
        }
    }
}