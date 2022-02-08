using OPADotNet.Reasons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.AspNetCore.Reasons
{
    internal class DefaultReasonFormatter : IReasonFormatter
    {
        private static readonly DefaultFormatterVisitor formatter = new DefaultFormatterVisitor();
        public string FormatReason(ReasonMessage reason)
        {
            return string.Join(Environment.NewLine, formatter.Visit(reason, 0));
        }
    }
}
