using System;
using Server.Lib.Models.Resources.Api;

namespace Server.Lib.Connectors.Http
{
    public interface IHttpClient
    {
        IHttpRequest Head(Uri target);
        IHttpRequest Get(Uri target);
    }
}