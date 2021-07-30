using OPADotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded
{
    public partial class OpaClientEmbedded : IOpaClient
    {
        private readonly OpaStore _opaStore;
        private List<WeakReference<PreparedPartialEmbedded>> preparedPartials = new List<WeakReference<PreparedPartialEmbedded>>();

        public OpaClientEmbedded(OpaStore opaStore)
        {
            _opaStore = opaStore;
        }

        public OpaStore OpaStore => _opaStore;

        internal void UpdatePrepared()
        {
            foreach(var preparedPartial in preparedPartials)
            {

            }
        }

        public IPreparedPartial PreparePartial(string query)
        {
            return new PreparedPartialEmbedded(_opaStore.GetCompiler(), OpaStore, query);
        }
    }
}
