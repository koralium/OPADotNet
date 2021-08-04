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
