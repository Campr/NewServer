using System;
using System.Collections.Generic;

namespace Server.Lib.Helpers
{
    public interface IHttpHelpers
    {
        IList<Uri> ReadLinksInHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, string rel);
    }
}