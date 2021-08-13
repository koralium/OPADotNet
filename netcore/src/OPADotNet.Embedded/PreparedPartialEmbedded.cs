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
using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OPADotNet.Embedded
{
    internal class PreparedPartialEmbedded : IPreparedPartial, IPreparedEmbedded
    {
        private static JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private int _preparedQueryId;
        private bool disposedValue;
        private readonly string _query;
        private readonly OpaStore _opaStore;

        internal PreparedPartialEmbedded(OpaStore opaStore, string query)
        {
            _query = query;
            _opaStore = opaStore;
            this.Update();
        }

        public Task<AstQueries> Partial(object input, IEnumerable<string> unknowns)
        {
            var unknownsArray = unknowns.ToArray();
            var inputJson = JsonSerializer.Serialize(input, serializerOptions);

            var t = new TaskCompletionSource<AstQueries>();
            int result = RegoWrapper.PreparedPartial(_preparedQueryId, inputJson, unknownsArray, unknownsArray.Length);

            if (result < 0)
            {
                string err = RegoWrapper.GetString(result);
                RegoWrapper.FreeString(result);
                t.SetException(new InvalidOperationException(err));
            }

            var content = RegoWrapper.GetString(result);

            t.SetResult(PartialJsonConverter.ReadPartialQuery(content));
            return t.Task;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                RegoWrapper.RemovePartialQuery(_preparedQueryId);
                disposedValue = true;
            }
        }

        ~PreparedPartialEmbedded()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void Update()
        {
            if (_preparedQueryId > 0)
            {
                RegoWrapper.RemovePartialQuery(_preparedQueryId);
            }
            int result = RegoWrapper.PreparePartial(_opaStore.GetCompiler().compilerId, _opaStore._storeId, _query);

            if (result < 0)
            {
                RegoWrapper.FreeString(result);
            }
            _preparedQueryId = result;
        }
    }
}
