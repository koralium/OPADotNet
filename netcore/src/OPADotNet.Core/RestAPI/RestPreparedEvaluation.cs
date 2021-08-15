using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Core.RestAPI
{
    internal class RestPreparedEvaluation : IPreparedEvaluation
    {
        private readonly RestOpaClient _restOpaClient;
        private readonly string _query;

        public RestPreparedEvaluation(RestOpaClient restOpaClient, string query)
        {
            _restOpaClient = restOpaClient;
            _query = query;
        }

        public void Dispose()
        {
            //Do nothing
        }

        public Task<IEnumerable<TBinding>> Evaluate<TBinding>(object input = null)
        {
            return _restOpaClient.Query<TBinding>(_query, input);
        }
    }
}
