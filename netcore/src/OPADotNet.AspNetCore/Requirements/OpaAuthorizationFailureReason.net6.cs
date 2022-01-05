using Microsoft.AspNetCore.Authorization;
using OPADotNet.Core.Ast.Explanation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore
{
    internal class OpaAuthorizationFailureReason : AuthorizationFailureReason
    {
        public IReadOnlyList<ExplanationNode> Explanation { get; }

        public OpaAuthorizationFailureReason(IAuthorizationHandler handler, string message, IReadOnlyList<ExplanationNode> explanation) : base(handler, message)
        {
            Explanation = explanation;
        }
    }
}
