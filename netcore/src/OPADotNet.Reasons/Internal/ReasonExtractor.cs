using OPADotNet.Ast.Models;
using OPADotNet.Core.Ast.Explanation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Reasons
{
    /// <summary>
    /// Extracts the reason from a query explanation.
    /// </summary>
    internal class ReasonExtractor
    {

        public ReasonMessage ExtractReason(ReasonQueryNode rootNode, List<ExplanationNode> explanationNodes)
        {
            HashSet<ExplanationLocation> enterNodes = new HashSet<ExplanationLocation>();
            List<ExplanationLocation> failNodes = new List<ExplanationLocation>();
            Dictionary<ExplanationLocation, List<object>> variables = new Dictionary<ExplanationLocation, List<object>>();

            foreach (var explanationNode in explanationNodes)
            {
                if (explanationNode.Operation == "enter")
                {
                    enterNodes.Add(explanationNode.Location);
                }
                else if (explanationNode.Operation == "fail")
                {
                    failNodes.Add(explanationNode.Location);
                }
                else if (explanationNode.Operation == "index" && explanationNode is ExplanationExpression expr)
                {
                    // Skip comparison terms
                    if (expr.Node.Terms.Count == 3 && expr.Node.Terms[0] is AstTermRef termRef && termRef.Value.Count == 1 && termRef.Value[0] is AstTermVar)
                    {
                        continue;
                    }
                    foreach(var term in expr.Node.Terms)
                    {
                        if (term is AstTermString termString)
                        {
                            if (!variables.TryGetValue(explanationNode.Location, out var variableList))
                            {
                                variableList = new List<object>();
                                variables.Add(explanationNode.Location, variableList);
                            }
                            variableList.Add(termString.Value);
                        }
                        else if (term is AstTermNumber termNumber) 
                        {
                            if (!variables.TryGetValue(explanationNode.Location, out var variableList))
                            {
                                variableList = new List<object>();
                                variables.Add(explanationNode.Location, variableList);
                            }
                            variableList.Add(termNumber.Value);
                        }
                        else if (term is AstTermBoolean termBoolean)
                        {
                            if (!variables.TryGetValue(explanationNode.Location, out var variableList))
                            {
                                variableList = new List<object>();
                                variables.Add(explanationNode.Location, variableList);
                            }
                            variableList.Add(termBoolean.Value);
                        }
                        else if (term is AstTermVar termVar)
                        {
                            if (!variables.TryGetValue(explanationNode.Location, out var variableList))
                            {
                                variableList = new List<object>();
                                variables.Add(explanationNode.Location, variableList);
                            }
                            var localNode = explanationNode.Locals.FirstOrDefault(x => x.Key is AstTermString astTermString && astTermString.Value == termVar.Value);
                            
                            // Do the check on localNode.Value to see if it is string, number etc to add it into the variable list.
                            //variableList.Add(termBoolean.Value);
                        }
                    }
                }
            }
            failNodes.Sort();
            var (reasonMessage, succeeded) = CheckNode(rootNode, enterNodes, failNodes, variables, null);

            return reasonMessage;
        }

        private (ReasonMessage, bool) CheckNode(ReasonQueryNode reasonNode, HashSet<ExplanationLocation> enterNodes, List<ExplanationLocation> failNodes, Dictionary<ExplanationLocation, List<object>> variablesLookup, IReadOnlyList<object>? variables)
        {
            if (!enterNodes.Contains(reasonNode.EnterLocation))
            {
                return (new ReasonMessage(reasonNode.Message), false);
            }

            int startIndex = failNodes.BinarySearch(reasonNode.EnterLocation);

            if (startIndex < 0)
            {
                int complement = ~startIndex;
                if (complement < failNodes.Count && failNodes[complement].File == reasonNode.FileName && failNodes[complement].Row >= reasonNode.StartLocation && failNodes[complement].Row <= reasonNode.EndLocation)
                {
                    startIndex = complement;
                }
            }
            
            if (startIndex < 0 || failNodes[startIndex].File != reasonNode.FileName)
            {
                return (new ReasonMessage(reasonNode.Message, variables), true);
            }

            List<ReasonAndCondition> children = new List<ReasonAndCondition>();
            for (int i = startIndex; i < failNodes.Count && failNodes[i].Row <= reasonNode.EndLocation; i++)
            {
                if (reasonNode.IndexNodes.TryGetValue(failNodes[i].Row, out var indexNodes))
                {
                    variablesLookup.TryGetValue(failNodes[i], out var variableList);
                    List<ReasonMessage> orConditions = new List<ReasonMessage>();
                    foreach(var indexNode in indexNodes)
                    {
                        var (reasonMessage, succeeded) = CheckNode(indexNode, enterNodes, failNodes, variablesLookup, variableList);

                        if (!succeeded)
                        {
                            orConditions.Add(reasonMessage);
                        }
                    }
                    children.Add(new ReasonAndCondition(orConditions));
                }
            }

            return (new ReasonMessage(reasonNode.Message, children, variables), false);
        }
    }
}
