using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    internal class SyncDataRefVisitor : AstVisitor<string>
    {
        public HashSet<string> DataSets { get; } = new HashSet<string>();
        public override string VisitTermRef(AstTermRef partialTermRef)
        {
            //Treat a new ref as a seperate object
            var dataVisitor = new SyncDataVisitor();
            dataVisitor.Visit(partialTermRef.Value);
            DataSets.UnionWith(dataVisitor.DataSets);

            //Replace the reference with a variable
            return "$0";
        }

        public override string VisitTermVar(AstTermVar partialTermVar)
        {
            //Skip printing out data
            if (partialTermVar.Value == "data")
            {
                return null;
            }
            //Return as a generic variable
            return "$0";
        }

        public override string VisitTermString(AstTermString partialTermString)
        {
            return partialTermString.Value;
        }

        public override string VisitTermBoolean(AstTermBoolean termBoolean)
        {
            return "$0";
        }

        public override string VisitTermNumber(AstTermNumber partialTermNumber)
        {
            return "$0";
        }
    }
}
