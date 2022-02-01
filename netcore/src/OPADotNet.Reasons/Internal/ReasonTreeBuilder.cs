using OPADotNet.Core.Ast.Explanation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Reasons
{
    internal class ReasonTreeBuilder
    {

        private Dictionary<string, Dictionary<int, string>> _messages;

        public ReasonTreeBuilder(Dictionary<string, Dictionary<int, string>> messages)
        {
            _messages = messages;
        }

        private class QueryNode
        {
            public int Id { get; set; }

            public Dictionary<string, ExplanationNode> Nodes { get; } = new Dictionary<string, ExplanationNode>();

            /// <summary>
            /// For each index operation in a query, a list exists of the expected query nodes that can be entered.
            /// </summary>
            public Dictionary<int, List<QueryNode>> IndexExpectedQueries { get; set; } = new Dictionary<int, List<QueryNode>>();

            public ExplanationNode CurrentNode { get; set; }

            public ExplanationNode IndexingNode { get; set; }

            public string Message { get; set; }

            public string FileName { get; set; }

            public int StartLocation { get; set; }

            public int EndLocation { get; set; }

            public string EnterLocation { get; set; }
        }

        public ReasonQueryNode AddExplanations(IEnumerable<ExplanationNode> nodes)
        {
            Dictionary<int, QueryNode> queryNodes = new Dictionary<int, QueryNode>();

            foreach (var node in nodes)
            {
                if (!queryNodes.TryGetValue(node.QueryId, out var queryNode))
                {
                    queryNode = new QueryNode()
                    {
                        Id = node.QueryId
                    };

                    queryNodes.Add(node.QueryId, queryNode);
                }

                var locationKey = $"{node.Location.File}:{node.Location.Row}:{node.Type}";

                if (node.Operation == "redo")
                {
                    if (queryNode.Nodes.TryGetValue(locationKey, out var existingNode))
                    {
                        queryNode.CurrentNode = existingNode;
                    }
                    continue;
                }

                if (node.Operation == "enter")
                {
                    queryNode.EnterLocation = $"{node.Location.File}:{node.Location.Row}";
                    queryNode.StartLocation = node.Location.Row;
                    queryNode.FileName = node.Location.File;

                    if (_messages.TryGetValue(node.Location.File, out var rowMessages) && rowMessages.TryGetValue(node.Location.Row - 1, out var message))
                    {
                        queryNode.Message = message;
                    }

                    if (queryNodes.TryGetValue(node.ParentId, out var parentQuery) && parentQuery?.CurrentNode?.Operation == "index")
                    {
                        //var indexKey = $"{parentQuery.CurrentNode.Location.File}:{parentQuery.CurrentNode.Location.Row}";
                        queryNode.IndexingNode = parentQuery.CurrentNode;
                        if (!parentQuery.IndexExpectedQueries.TryGetValue(parentQuery.CurrentNode.Location.Row, out var indexQueryNodes))
                        {
                            indexQueryNodes = new List<QueryNode>();
                            parentQuery.IndexExpectedQueries.Add(parentQuery.CurrentNode.Location.Row, indexQueryNodes);
                        }
                        indexQueryNodes.Add(queryNode);
                    }
                }

                if (queryNode.EndLocation < node.Location.Row)
                {
                    queryNode.EndLocation = node.Location.Row;
                }

                // Skip adding eval and save nodes
                if (node.Operation == "eval" || node.Operation == "save")
                {
                    continue;
                }

                if (!queryNode.Nodes.ContainsKey(locationKey))
                {
                    queryNode.Nodes.Add(locationKey, node);
                }

                queryNode.CurrentNode = node;
            }

            Dictionary<int, ReasonQueryNode> reasonQueryNodes = new Dictionary<int, ReasonQueryNode>();
            foreach(var queryNode in queryNodes)
            {
                var reasonQueryNode = new ReasonQueryNode(
                    queryNode.Value.Message, 
                    queryNode.Value.FileName, 
                    queryNode.Value.StartLocation, 
                    queryNode.Value.EndLocation,
                    new ExplanationLocation()
                    {
                        File = queryNode.Value.FileName,
                        Row = queryNode.Value.StartLocation,
                    });
                reasonQueryNodes.Add(queryNode.Key, reasonQueryNode);
            }

            foreach (var queryNode in queryNodes)
            {
                var reasonQueryNode = reasonQueryNodes[queryNode.Key];
                foreach(var indexNodes in queryNode.Value.IndexExpectedQueries)
                {
                    foreach(var indexNode in indexNodes.Value)
                    {
                        if (!reasonQueryNodes.TryGetValue(indexNode.Id, out var indexedQueryNode))
                        {
                            throw new InvalidOperationException("Could not find the a query node for the query id: " + indexNode.Id);
                        }
                        if (!reasonQueryNode.IndexNodes.TryGetValue(indexNodes.Key, out var indexNodeList))
                        {
                            indexNodeList = new List<ReasonQueryNode>();
                            reasonQueryNode.IndexNodes.Add(indexNodes.Key, indexNodeList);
                        }
                        indexNodeList.Add(indexedQueryNode);
                    }
                }
            }

            return reasonQueryNodes[0];
        }
    }
}
