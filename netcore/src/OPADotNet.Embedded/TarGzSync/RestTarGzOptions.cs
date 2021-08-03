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
using OPADotNet.Embedded.TarGzSync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Sync
{
    public class RestTarGzOptions
    {
        public Uri Url { get; set; }

        public TimeSpan Interval { get; set; }

        public IDictionary<string, string> Headers { get; }

        internal RestCredentialMethod CredentialMethod { get; set; }

        public RestTarGzOptions UseOAuth2(Action<OAuth2CredentialMethod> options)
        {
            OAuth2CredentialMethod oAuth2CredentialMethod = new OAuth2CredentialMethod();
            options?.Invoke(oAuth2CredentialMethod);
            CredentialMethod = oAuth2CredentialMethod;
            return this;
        }

        public RestTarGzOptions()
        {
            Headers = new Dictionary<string, string>();
        }

        public override bool Equals(object obj)
        {
            if (obj is RestTarGzOptions other)
            {
                return Equals(Url, other.Url) &&
                    Equals(Interval, other.Interval) &&
                    Equals(CredentialMethod, other.CredentialMethod);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Url, Interval, CredentialMethod);
        }
    }
}
