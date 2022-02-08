using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Reasons
{
    public abstract class ReasonNode
    {
        public abstract TReturn Accept<TReturn, TState>(ReasonVisitor<TReturn, TState> visitor, TState state = default);
    }
}
