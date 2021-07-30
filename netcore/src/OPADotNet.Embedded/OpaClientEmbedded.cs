using OPADotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded
{
    public partial class OpaClientEmbedded : IOpaClient
    {
        private readonly OpaStore _opaStore;
        private readonly List<WeakReference<PreparedPartialEmbedded>> _preparedPartials = new List<WeakReference<PreparedPartialEmbedded>>();

        public OpaClientEmbedded()
        {
            _opaStore = new OpaStore(this);
        }

        public OpaStore OpaStore => _opaStore;

        internal void UpdatePrepared()
        {
            for (int i = 0; i < _preparedPartials.Count; i++)
            {
                if (_preparedPartials[i].TryGetTarget(out var target))
                {
                    target.Update();
                }
                else
                {
                    _preparedPartials.RemoveAt(i);
                    i--;
                }
            }
        }

        public IPreparedPartial PreparePartial(string query)
        {
            var preparedPartial = new PreparedPartialEmbedded(OpaStore, query);
            _preparedPartials.Add(new WeakReference<PreparedPartialEmbedded>(preparedPartial));
            return preparedPartial;
        }
    }
}
