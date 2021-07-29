using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.RestAPI
{
    internal class RestPreparedPartial : IPreparedPartial
    {
        private readonly RestOpaClient _restOpaClient;
        private readonly string _query;

        public RestPreparedPartial(RestOpaClient restOpaClient, string query)
        {
            _restOpaClient = restOpaClient;
            _query = query;
        }

        public void Dispose()
        {
            //Nothing to dispose
        }

        public Task<AstQueries> Partial(object input, IEnumerable<string> unknowns)
        {
            return _restOpaClient.Compile(_query, input, unknowns.ToList());
        }
    }
}
