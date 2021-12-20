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

        public OpaCompiler GetCompiler()
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
