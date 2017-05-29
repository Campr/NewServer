using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Server.Lib.Extensions;
using Server.Lib.Helpers;
using Server.Lib.Infrastructure;

namespace Server.Lib.Models.Other
{
    class HawkSignatureFactory : IHawkSignatureFactory
    {
        public HawkSignatureFactory(
            ICryptoHelpers cryptoHelpers,
            ITextHelpers textHelpers,
            IUriHelpers uriHelpers)
        {
            Ensure.Argument.IsNotNull(cryptoHelpers, nameof(cryptoHelpers));
            Ensure.Argument.IsNotNull(textHelpers, nameof(textHelpers));
            Ensure.Argument.IsNotNull(uriHelpers, nameof(uriHelpers));

            this.cryptoHelpers = cryptoHelpers;
            this.textHelpers = textHelpers;
            this.uriHelpers = uriHelpers;

            this.authorizationHeaderRegex = new Regex("(id|ts|nonce|mac|ext|hash|app)=\"([^\"\\\\]*)\"");
        }

        private readonly ICryptoHelpers cryptoHelpers;
        private readonly ITextHelpers textHelpers;
        private readonly IUriHelpers uriHelpers;

        private readonly Regex authorizationHeaderRegex;

        public IHawkSignature FromAuthorizationHeader(string header)
        {
            Ensure.Argument.IsNotNullOrWhiteSpace(header, nameof(header));
            
            // Extract the various values of the Hawk signature from the Authorization header.
            var matches = this.authorizationHeaderRegex.Matches(header);
            var matchDictionary = new Dictionary<string, string>();
            foreach (Match match in matches)
                matchDictionary[match.Groups[1].Value] = match.Groups[2].Value;

            // Make sure we have the correct values.
            if (!matchDictionary.ContainsKey("id")
                || !matchDictionary.ContainsKey("ts")
                || !matchDictionary.ContainsKey("nonce")
                || !matchDictionary.ContainsKey("mac"))
                throw new Exception("The provided string is not a valid Hawk authorization header.");

            // Create the signature from the values we found.
            return new HawkSignature(this.cryptoHelpers, this.textHelpers, this.uriHelpers)
            {
                Id = matchDictionary["id"],
                Timestamp = long.Parse(matchDictionary["ts"]).FromSecondTime(),
                Nonce = matchDictionary["nonce"],
                Mac = matchDictionary["mac"],
                ContentHash = matchDictionary.TryGetValue("hash"),
                Extension = matchDictionary.TryGetValue("ext", string.Empty),
                App = matchDictionary.TryGetValue("app"),
                Type = HawkMacType.Header
            };
        }

        public IHawkSignature FromBewit(string bewit)
        {
            // Fix the padding of the bewit string.
            if (bewit.Length % 4 > 0)
                bewit = bewit.PadRight(bewit.Length + 4 - bewit.Length % 4, '=');

            // Read the actual string.
            var bewitValue = Encoding.UTF8.GetString(Convert.FromBase64String(bewit));

            // Parse it.
            var bewitParts = bewitValue.Split('\\');
            if (bewitParts.Length != 4)
                throw new Exception("The provided string is not a valid Hawk bewit value (incorrect number of parts).");

            // Parse the timestamp.
            if (!long.TryParse(bewitParts[1], out var unixDate))
                throw new Exception("The provided string is not a valid Hawk bewit value (invalid timestamp).");

            // Create the signature from the values we found.
            return new HawkSignature(this.cryptoHelpers, this.textHelpers, this.uriHelpers)
            {
                Id = bewitParts[0],
                Timestamp = unixDate.FromSecondTime(),
                Nonce = string.Empty,
                Mac = bewitParts[2],
                Extension = bewitParts[3],
                Type = HawkMacType.Bewit
            };
        }

        //public ITentHawkSignature FromCredentials(TentPost<TentContentCredentials> credentials)
        //{
        //    return new TentHawkSignature(this.cryptoHelpers, this.textHelpers, this.uriHelpers)
        //    {
        //        Id = credentials.Id,
        //        Key = Encoding.UTF8.GetBytes(credentials.Content.HawkKey),
        //        Type = HawkMacType.Header
        //    };
        //}
    }
}