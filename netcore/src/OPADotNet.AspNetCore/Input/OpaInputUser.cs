﻿/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.AspNetCore.Input
{
    /// <summary>
    /// The user object that will be sent to OPA
    /// </summary>
    class OpaInputUser
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("claims")]
        public Dictionary<string, List<string>> Claims { get; set; }

        [JsonPropertyName("isAuthenticated")]
        public bool IsAuthenticated { get; set; }

        public static OpaInputUser FromPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            var output = new OpaInputUser()
            {
                Name = claimsPrincipal?.Identity?.Name,
                Claims = new Dictionary<string, List<string>>(),
                IsAuthenticated = claimsPrincipal?.Identity?.IsAuthenticated ?? false
            };

            if (claimsPrincipal?.Claims != null)
            {
                foreach (var claim in claimsPrincipal.Claims)
                {
                    if (!output.Claims.TryGetValue(claim.Type, out var claims))
                    {
                        claims = new List<string>();
                        output.Claims.Add(claim.Type, claims);
                    }
                    claims.Add(claim.Value);
                }
            }

            return output;
        }
    }
}
