/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
