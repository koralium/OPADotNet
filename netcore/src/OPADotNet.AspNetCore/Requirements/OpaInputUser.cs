using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.AspNetCore.Requirements
{
    /// <summary>
    /// The user object that will be sent to OPA
    /// </summary>
    class OpaInputUser
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("claims")]
        public List<OpaInputUserClaim> Claims { get; set; }

        public static OpaInputUser FromPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                return null;
            }
            var output = new OpaInputUser()
            {
                Name = claimsPrincipal.Identity.Name,
                Claims = new List<OpaInputUserClaim>()
            };

            foreach (var claim in claimsPrincipal.Claims)
            {
                output.Claims.Add(OpaInputUserClaim.FromClaim(claim));
            }

            return output;
        }
    }
}
