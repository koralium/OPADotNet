using OPADotNet.Core.Ast.Explanation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Reasons
{
    internal class ReasonQueryNode
    {
        /// <summary>
        /// The message for the user
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Filename that the node exist in
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Starting location where the query node starts
        /// </summary>
        public int StartLocation { get; }

        /// <summary>
        /// End location for the query node
        /// </summary>
        public int EndLocation { get; }

        public ExplanationLocation EnterLocation { get; }

        /// <summary>
        /// Contains index lookups and the query nodes that are entered for that index operation.
        /// </summary>
        public Dictionary<int, List<ReasonQueryNode>> IndexNodes { get; }

        public ReasonQueryNode(string message, string fileName, int startLocation, int endLocation, ExplanationLocation enterLocation)
        {
            Message = message;
            FileName = fileName;
            StartLocation = startLocation;
            EndLocation = endLocation;
            EnterLocation = enterLocation;
            IndexNodes = new Dictionary<int, List<ReasonQueryNode>>();
        }
    }
}
