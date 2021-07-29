using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    internal class SyncPathVisitor : AstVisitor<string>
    {
        public override string VisitPolicyPackage(AstPolicyPackage astPolicyPackage)
        {
            return string.Join(".", Visit(astPolicyPackage.Path).Where(x => x != null));
        }

        public override string VisitTermString(AstTermString partialTermString)
        {
            return partialTermString.Value;
        }
    }
}
