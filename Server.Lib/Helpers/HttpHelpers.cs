using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Server.Lib.Extensions;

namespace Server.Lib.Helpers
{
    class HttpHelpers : IHttpHelpers
    {
        public IList<Uri> ReadLinksInHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, string rel)
        {
            var result = new List<Uri>();

            // Find the "link" header.
            var headersDictionary = headers.ToDictionary(kv => kv.Key.ToLower(), kv => kv.Value);
            var links = headersDictionary.TryGetValue("link");
            if (links == null)
                return result;
            
            // Create a RegEx to parse the link we're looking for.
            var linkRegex = new Regex(string.Format(
                CultureInfo.InvariantCulture,
                "<(.*)>; rel=\"{0}\"",
                rel));

            // Parse the links as URIs.
            result.AddRange(links
                .Select(l => linkRegex.Match(l))
                .Where(m => m.Success)
                .Select(m => new Uri(m.Groups[1].Value, UriKind.RelativeOrAbsolute)));

            return result;
        }
    }
}