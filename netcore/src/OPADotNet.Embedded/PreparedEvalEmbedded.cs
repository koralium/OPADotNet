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
using OPADotNet.Embedded.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OPADotNet.Embedded
{
    internal class PreparedEvalEmbedded : IPreparedEvaluation, IPreparedEmbedded
    {
        private static object emptyInput = new object();

        private int _preparedEvalId;
        private bool disposedValue;
        private readonly string _query;
        private readonly OpaStore _opaStore;

        public PreparedEvalEmbedded(OpaStore opaStore, string query)
        {
            _opaStore = opaStore;
            _query = query;
            Update();
        }

        public void Update()
        {
            if (_preparedEvalId > 0)
            {
                RegoWrapper.RemoveEvalQuery(_preparedEvalId);
            }
            int result = RegoWrapper.PrepareEvaluation(_opaStore.GetCompiler().compilerId, _opaStore._storeId, _query);

            if (result < 0)
            {
                RegoWrapper.FreeString(result);
            }
            _preparedEvalId = result;
        }

        public Task<IEnumerable<TBinding>> Evaluate<TBinding>(object input = null)
        {
            if (input == null)
            {
                input = emptyInput;
            }

            var inputJson = JsonSerializer.Serialize(input);

            var t = new TaskCompletionSource<IEnumerable<TBinding>>();
            int result = RegoWrapper.PreparedEval(_preparedEvalId, inputJson);

            if (result < 0)
            {
                string err = RegoWrapper.GetString(result);
                RegoWrapper.FreeString(result);
                t.SetException(new InvalidOperationException(err));
            }

            var content = RegoWrapper.GetString(result);

            if (content == null || content == "null")
            {
                return null;
            }

            var evaluateResult = JsonSerializer.Deserialize<List<EvaluateResult<TBinding>>>(content);
            t.SetResult(evaluateResult.Select(x => x.Bindings));
            return t.Task;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                RegoWrapper.RemoveEvalQuery(_preparedEvalId);
                disposedValue = true;
            }
        }

        ~PreparedEvalEmbedded()
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
