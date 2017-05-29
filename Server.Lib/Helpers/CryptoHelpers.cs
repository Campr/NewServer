using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Server.Lib.Extensions;

namespace Server.Lib.Helpers
{
    class CryptoHelpers : ICryptoHelpers
    {
        public string CreateBewit(DateTime expiresAt, Uri uri, string ext, string bewitId, byte[] key)
        {
            // Create the mac hash.
            var mac = this.CreateMac("bewit", expiresAt, null, "GET", uri, null, ext, null, key);

            // Use it to create the bewit.
            var bewit = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\{2}\\{3}", bewitId, expiresAt.ToSecondTime(), mac, ext);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(bewit)).TrimEnd('=');
        }

        public string CreateMac(string header, DateTime timestamp, string nonce, string verb, Uri uri, string contentHash, string ext, string app, byte[] key)
        {
            // Escape the ext string.
            if (!string.IsNullOrEmpty(ext))
            {
                ext = ext.Replace("\\", "\\\\").Replace("\n", "\\n");
            }

            // Create our version of the hash.
            var hashContent = new List<string>
            {
                "hawk.1." + header,
                timestamp.ToSecondTime().ToString(CultureInfo.InvariantCulture),
                nonce,
                verb.ToUpper(),
                uri.PathAndQuery,
                uri.Host,
                uri.Port.ToString(CultureInfo.InvariantCulture),
                contentHash,
                ext
            };

            // If needed, add the App parameter.
            if (!string.IsNullOrEmpty(app))
            {
                hashContent.Add(app);
                hashContent.Add(string.Empty);
            }

            // Trailing breaks.
            hashContent.Add(string.Empty);

            var hashContentStr = string.Join("\n", hashContent);

            // Hash it using the HMAC256 algorithm.
            using (var hmac = new HMACSHA256(key))
            {
                var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashContentStr));
                return Convert.ToBase64String(hashValue);
            }
        }

        public string CreateStaleTimestampMac(DateTime timestamp, byte[] key)
        {
            // Create the unashed version of the mac.
            var hashContent = new List<string>
            {
                "hawk.1.ts",
                timestamp.ToSecondTime().ToString(CultureInfo.InvariantCulture)
            };

            var hashContentStr = string.Join("\n", hashContent);

            // Hash it using the HMAC256 algorithm.
            using (var hmac = new HMACSHA256(key))
            {
                var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashContentStr));
                return Convert.ToBase64String(hashValue);
            }
        }
    }
}