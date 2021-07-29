using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.AspNetCore.Requirements
{
    /// <summary>
    /// Contains a single claim for a user
    /// </summary>
    class OpaInputUserClaim
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        public static OpaInputUserClaim FromClaim(Claim claim)
        {
            return new OpaInputUserClaim()
            {
                Type = claim.Type,
                Value = claim.Value
            };
        }
    }
}
