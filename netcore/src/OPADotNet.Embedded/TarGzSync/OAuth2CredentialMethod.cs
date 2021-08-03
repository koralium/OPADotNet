/*
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
using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using IdentityModel;
using IdentityModel.Client;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.TarGzSync
{
    public class OAuth2CredentialMethod : RestCredentialMethod
    {
        public string TokenUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public List<string> Scopes { get; set; }

        internal override async Task Apply(HttpRequestMessage httpRequestMessage)
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = TokenUrl,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Scope = string.Join(" ", Scopes)
            });

            if (result.IsError)
            {
                throw new HttpRequestException("Could not get an access token, error: " + result.Error);
            }

            //Set the access token
            httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
        }

        public override bool Equals(object obj)
        {
            if (obj is OAuth2CredentialMethod other)
            {
                return Equals(TokenUrl, other.TokenUrl) &&
                    Equals(ClientId, other.ClientId) &&
                    Equals(ClientSecret, other.ClientSecret) &&
                    Scopes.AreEqual(other.Scopes);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(TokenUrl);
            hashCode.Add(ClientId);
            hashCode.Add(ClientSecret);

            if (Scopes != null)
            {
                foreach (var scope in Scopes)
                {
                    hashCode.Add(scope);
                }
            }
            
            return hashCode.ToHashCode();
        }
    }
}
