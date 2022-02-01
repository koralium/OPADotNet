using System;
using System.Collections.Generic;
using System.Text;
using OPADotNet.Core.Extensions;

namespace OPADotNet.Reasons
{
    public class ReasonMessage : ReasonNode
    {
        private static readonly List<ReasonAndCondition> _emptyList = new List<ReasonAndCondition>();
        private static readonly List<object> _emptyVariableList = new List<object>();

        public string Message { get; }

        public IReadOnlyList<ReasonAndCondition> AndConditions { get; }

        /// <summary>
        /// Possible variables pushed down from the index function, this can for example be function parameters.
        /// </summary>
        public IReadOnlyList<object> Variables { get; }

        public ReasonMessage(string message, IReadOnlyList<ReasonAndCondition> andConditions, IReadOnlyList<object>? variables)
        {
            Message = message;
            AndConditions = andConditions;
            if (variables != default)
            {
                Variables = variables;
            }
            else
            {
                Variables = _emptyVariableList;
            }
        }

        public ReasonMessage(string message, IReadOnlyList<ReasonAndCondition> andConditions)
        {
            Message = message;
            AndConditions = andConditions;
            Variables = _emptyVariableList;
        }

        public ReasonMessage(string message)
        {
            Message = message;
            AndConditions = _emptyList;
            Variables = _emptyVariableList;
        }

        public ReasonMessage(string message, IReadOnlyList<object>? variables)
        {
            Message = message;
            AndConditions = _emptyList;
            if (variables != default)
            {
                Variables = variables;
            }
            else
            {
                Variables = _emptyVariableList;
            }
        }

        public override TReturn Accept<TReturn, TState>(ReasonVisitor<TReturn, TState> visitor, TState state)
        {
            return visitor.VisitReasonMessage(this, state);
        }

        public override bool Equals(object obj)
        {
            if (obj is ReasonMessage other)
            {
                return Equals(Message, other.Message) && AndConditions.AreEqual(other.AndConditions) && Variables.AreEqual(other.Variables);
            }
            
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Message);
            
            foreach(var andCondition in AndConditions)
            {
                hashCode.Add(andCondition);
            }

            foreach (var variable in Variables)
            {
                hashCode.Add(variable);
            }

            return hashCode.ToHashCode();
        }
    }
}
