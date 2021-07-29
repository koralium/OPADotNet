using OPADotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded
{
    public partial class OpaClientEmbedded : IOpaClient
    {
        private readonly OpaStore _opaStore;

        public OpaClientEmbedded( OpaStore opaStore)
        {
            _opaStore = opaStore;
        }

        public OpaStore OpaStore => _opaStore;

        public IPreparedPartial PreparePartial(string query)
        {
            return new PreparedPartialEmbedded(_opaStore.GetCompiler(), OpaStore, query);
        }
    }
}
