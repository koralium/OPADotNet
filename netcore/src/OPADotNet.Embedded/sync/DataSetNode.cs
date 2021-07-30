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
