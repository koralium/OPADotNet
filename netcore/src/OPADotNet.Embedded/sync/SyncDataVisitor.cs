using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    internal class SyncDataVisitor : AstVisitor<string>
    {
        private HashSet<string> dataSets = new HashSet<string>();

        public HashSet<string> DataSets => dataSets;

        public override string VisitTermRef(AstTermRef partialTermRef)
        {
            if (partialTermRef.Value.Count > 1 && partialTermRef.Value[0] is AstTermVar termVar &&
                termVar.Value == "data")
            {
                SyncDataRefVisitor syncDataRefVisitor = new SyncDataRefVisitor();
                var dataSet = syncDataRefVisitor.Visit(partialTermRef.Value);
                dataSets.Add(string.Join(".", dataSet.Where(x => x != null)));
                dataSets.UnionWith(syncDataRefVisitor.DataSets);
            }
            
            return null;
        }
    }
}
