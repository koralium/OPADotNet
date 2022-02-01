using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Reasons
{
    public class ReasonAndCondition : ReasonNode
    {
        public IReadOnlyList<ReasonMessage> OrConditions { get; }

        public ReasonAndCondition(IReadOnlyList<ReasonMessage> orConditions)
        {
            OrConditions = orConditions;
        }

        public override TReturn Accept<TReturn, TState>(ReasonVisitor<TReturn, TState> visitor, TState state)
        {
            return visitor.VisitReasonAndCondition(this, state);
        }

        public override bool Equals(object obj)
        {
            if (obj is ReasonAndCondition other)
            {
                return OrConditions.AreEqual(other.OrConditions);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            foreach (var orCondition in OrConditions)
            {
                hashCode.Add(orCondition);
            }

            return hashCode.ToHashCode();
        }
    }
}
