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

        public OpaStore()
        {
            _storeId = RegoWrapper.NewStore();
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
        }

        public OpaTransaction NewTransaction()
        {
            var txnId = RegoWrapper.NewTransaction(_storeId);
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
