using OPADotNet.Core.Ast.Explanation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Reasons
{
    public interface IReasonHandler
    {
        Task Load(IEnumerable<string> queries);

        bool TryGetReasonMessage(string query, List<ExplanationNode> explanations, [NotNullWhen(returnValue: true)] out ReasonMessage? reasonMessage);
    }
}
