using OPADotNet.Embedded.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OPADotNet.Embedded
{
    public class OpaStore : IDisposable
    {
        internal readonly int _storeId;
        private bool disposedValue;
        private OpaCompiler _opaCompiler;
        internal IReadOnlyDictionary<string, string> _modules = new Dictionary<string, string>();
        private readonly OpaClientEmbedded _opaClientEmbedded;

        public OpaStore(OpaClientEmbedded opaClientEmbedded)
        {
            _storeId = RegoWrapper.NewStore();
            _opaClientEmbedded = opaClientEmbedded;
        }

        internal OpaCompiler GetCompiler()
        {
            if (_opaCompiler == null)
            {
                _opaCompiler = new OpaCompiler(_modules);
            }
            return _opaCompiler;
        }

        internal void NewCompiler()
        {
            _opaCompiler = new OpaCompiler(_modules);
            _opaClientEmbedded.UpdatePrepared();
        }

        public OpaTransaction NewTransaction(bool write)
        {
            int writeVal = write ? 1 : 0;
            var txnId = RegoWrapper.NewTransaction(_storeId, writeVal);
            return new OpaTransaction(this, txnId);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                RegoWrapper.RemoveStore(_storeId);
                disposedValue = true;
            }
        }

        ~OpaStore()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
