using OPADotNet.Ast.Models;
using OPADotNet.Core.Ast.Explanation;
using OPADotNet.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Core.Models
{
    public class PartialResult
    {
        public AstQueries Result { get; set; }

        public List<ExplanationNode> Explanation { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is PartialResult other)
            {
                return Equals(Result, other.Result) &&
                    Explanation.AreEqual(other.Explanation);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(Result);

            foreach (var explanation in Explanation)
            {
                hashCode.Add(explanation);
            }

            return hashCode.ToHashCode();
        }
    }
}
