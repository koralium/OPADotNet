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
    internal class PreparedEvalEmbedded : IDisposable, IPreparedEmbedded
    {
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

        public Task<TBinding> Evaluate<TBinding>(object input)
        {
            var inputJson = JsonSerializer.Serialize(input);

            var t = new TaskCompletionSource<TBinding>();
            int result = RegoWrapper.PreparedEval(_preparedEvalId, inputJson);

            if (result < 0)
            {
                string err = RegoWrapper.GetString(result);
                RegoWrapper.FreeString(result);
                t.SetException(new InvalidOperationException(err));
            }

            var content = RegoWrapper.GetString(result);

            var evaluateResult = JsonSerializer.Deserialize<List<EvaluateResult<TBinding>>>(content);
            t.SetResult(evaluateResult.First().Bindings);
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
