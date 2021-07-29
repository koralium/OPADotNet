using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    public class OpaBuilder
    {
        private string _opaServerUrl;
        private bool _useEmbedded = true;
        private TimeSpan _syncTime;

        /// <summary>
        /// Contains policies that will be compiled if running in the embedded mode
        /// </summary>
        public List<OpaBuilderPolicy> Policies { get; } = new List<OpaBuilderPolicy>();

        /// <summary>
        /// Add a connection to an OPA server.
        /// 
        /// If using embedded mode, policies and data will be collected from the OPA server.
        /// If sync time is set, it will sync policies and data on that interval
        /// </summary>
        /// <param name="url"></param>
        public OpaBuilder OpaServer(string url, TimeSpan syncTime = default(TimeSpan))
        {
            _opaServerUrl = url;
            _syncTime = syncTime;
            return this;
        }

        /// <summary>
        /// If using embedded mode, this will add a policy
        /// </summary>
        /// <param name="policyName"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public OpaBuilder AddPolicy(string policyName, string module)
        {
            Policies.Add(new OpaBuilderPolicy(policyName, module));
            return this;
        }

        /// <summary>
        /// Should embedded mode be used? Default is true
        /// </summary>
        /// <param name="useEmbedded"></param>
        /// <returns></returns>
        public OpaBuilder UseEmbedded(bool useEmbedded = true)
        {
            _useEmbedded = useEmbedded;
            return this;
        }

        internal OpaOptions Build()
        {
            if (!_useEmbedded && Policies.Count > 0)
            {
                throw new InvalidOperationException("Cannot use provided policies without embedded mode.");
            }

            Uri url = null;
            if (_opaServerUrl != null)
            {
                url = new Uri(_opaServerUrl);
            }

            return new OpaOptions(url, Policies, _useEmbedded, _syncTime);
        }
    }
}
