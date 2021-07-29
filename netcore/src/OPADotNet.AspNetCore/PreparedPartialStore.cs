using OPADotNet.AspNetCore.Requirements;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore
{
    /// <summary>
    /// Contains prepared partials
    /// </summary>
    internal class PreparedPartialStore
    {
        private readonly IOpaClient _opaClient;
        private readonly Dictionary<int, IPreparedPartial> _preparedPartials = new Dictionary<int, IPreparedPartial>();
        public PreparedPartialStore(IOpaClient opaClient)
        {
            _opaClient = opaClient;
        }

        public void PreparePartial(OpaPolicyRequirement opaPolicyRequirement)
        {
            _preparedPartials.Add(opaPolicyRequirement.Index, _opaClient.PreparePartial(opaPolicyRequirement.GetQuery()));
        }

        public IPreparedPartial GetPreparedPartial(OpaPolicyRequirement opaPolicyRequirement)
        {
            if (_preparedPartials.TryGetValue(opaPolicyRequirement.Index, out var preparedPartial))
            {
                return preparedPartial;
            }
            throw new InvalidOperationException("The opa policy requirements did not have any prepared partial");
        }
    }
}
