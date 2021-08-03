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
