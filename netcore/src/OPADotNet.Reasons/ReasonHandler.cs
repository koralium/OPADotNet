using OPADotNet.Core.Ast.Explanation;
using OPADotNet.Embedded;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Reasons
{
    public class ReasonHandler : IReasonHandler
    {
        private static readonly ReasonExtractor reasonExtractor = new ReasonExtractor();
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly Dictionary<string, ReasonQueryNode> _reasons = new Dictionary<string, ReasonQueryNode>();

        public ReasonHandler(OpaClientEmbedded opaClientEmbedded)
        {
            _opaClientEmbedded = opaClientEmbedded;
        }

        /// <summary>
        /// Loads in policies from the embedded storage, this must be called if any policy changes has been made.
        /// A list of queries must be supplied that contains the queries to create reasons for.
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        public async Task Load(IEnumerable<string> queries)
        {
            var policies = await _opaClientEmbedded.GetPolicies();
            var policyReasons = PolicyReasonExtractor.ExtractReasons(policies);
            ReasonTreeBuilder reasonTreeBuilder = new ReasonTreeBuilder(policyReasons);

            foreach(var query in queries)
            {
                var fullPartialResult = await _opaClientEmbedded.FullPartial(query);
                var reasonNode = reasonTreeBuilder.AddExplanations(fullPartialResult.Explanation);

                if (!_reasons.ContainsKey(query))
                {
                    _reasons.Add(query, reasonNode);
                }
                else
                {
                    reasonNode = _reasons[query];
                }
            }
        }

        public bool TryGetReasonMessage(string query, List<ExplanationNode> explanations, [NotNullWhen(returnValue: true)] out ReasonMessage? reasonMessage)
        {
            if (_reasons.TryGetValue(query, out ReasonQueryNode reasonNode))
            {
                reasonMessage = reasonExtractor.ExtractReason(reasonNode, explanations);
                return true;
            }
            reasonMessage = default;
            return false;
        }
    }
}
