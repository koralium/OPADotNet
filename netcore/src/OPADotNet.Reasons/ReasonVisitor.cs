using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Reasons
{
    public class ReasonVisitor<TReturn, TState>
    {
        public TReturn Visit(ReasonNode node, TState state = default)
        {
            return node.Accept(this, state);
        }

        public IEnumerable<TReturn> Visit(IEnumerable<ReasonNode> nodes, TState state = default)
        {
            return nodes.Select(x => x.Accept(this, state));
        }

        public virtual TReturn VisitReasonMessage(ReasonMessage reasonMessage, TState state = default)
        {
            Visit(reasonMessage.AndConditions, state);
            return default;
        }

        public virtual TReturn VisitReasonAndCondition(ReasonAndCondition reasonAndCondition, TState state = default)
        {
            Visit(reasonAndCondition.OrConditions, state);
            return default;
        }
    }
}
