using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    internal static class DataSetTreeBuilder
    {
        public static DataSetNode BuildDataSetTree(IEnumerable<string> dataSetPaths)
        {
            DataSetNode root = new DataSetNode("data", false);
            
            foreach (var dataSetPath in dataSetPaths)
            {
                var members = dataSetPath.Split('.');
                AddToTree(members, 0, root);
            }
            return root;
        }

        private static void AddToTree(string[] members, int index, DataSetNode node)
        {
            if (members.Length == index)
            {
                node.UsedInPolicyMutable = true;
                return;
            }

            if (!node.ChildrenMutable.TryGetValue(members[index], out var childNode))
            {
                bool isVariable = false;
                if (members[index] == "$0")
                {
                    isVariable = true;
                }
                childNode = new DataSetNode(members[index], isVariable);
                node.ChildrenMutable.Add(members[index], childNode);
            }
            AddToTree(members, index + 1, childNode);
        }
    }
}
