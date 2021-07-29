using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    /// <summary>
    /// This visitor reads a policy and figures out its name and the data it references.
    /// </summary>
    internal class SyncPolicyVisitor : AstVisitor<SyncPolicy>
    {
        private static SyncPathVisitor pathVisitor = new SyncPathVisitor();
        public override SyncPolicy VisitPolicy(AstPolicy astPolicy)
        {
            string path = pathVisitor.Visit(astPolicy.Package);

            SyncDataVisitor syncDataVisitor = new SyncDataVisitor();
            syncDataVisitor.Visit(astPolicy.Rules);

            return new SyncPolicy()
            {
                DataSets = syncDataVisitor.DataSets,
                PolicyName = path
            };
        }
    }
}
