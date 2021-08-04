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
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    public class DataSetNode
    {
        public string Name { get; }

        public bool IsVariable { get; }

        internal Dictionary<string, DataSetNode> ChildrenMutable { get; }

        internal bool UsedInPolicyMutable { get; set; }

        public bool UsedInPolicy => UsedInPolicyMutable;

        public IReadOnlyDictionary<string, DataSetNode> Children => ChildrenMutable;

        public DataSetNode(string name, bool isVariable)
        {
            Name = name;
            IsVariable = isVariable;
            ChildrenMutable = new Dictionary<string, DataSetNode>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
