using OPADotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded
{
    public partial class OpaClientEmbedded : IOpaClient
    {
        private readonly OpaStore _opaStore;
        private readonly List<WeakReference<IPreparedEmbedded>> _prepared = new List<WeakReference<IPreparedEmbedded>>();

        public OpaClientEmbedded()
        {
            _opaStore = new OpaStore(this);
        }

        public OpaStore OpaStore => _opaStore;

        internal void UpdatePrepared()
        {
            for (int i = 0; i < _prepared.Count; i++)
            {
                if (_prepared[i].TryGetTarget(out var target))
                {
                    target.Update();
                }
                else
                {
                    _prepared.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Internal for now until the API is more understood
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal PreparedEvalEmbedded PrepareEvaluation(string query)
        {
            var preparedEval = new PreparedEvalEmbedded(OpaStore, query);
            _prepared.Add(new WeakReference<IPreparedEmbedded>(preparedEval));
            return preparedEval;
        }

        public IPreparedPartial PreparePartial(string query)
        {
            var preparedPartial = new PreparedPartialEmbedded(OpaStore, query);
            _prepared.Add(new WeakReference<IPreparedEmbedded>(preparedPartial));
            return preparedPartial;
        }
    }
}
