using Microsoft.Extensions.Logging;
using OPADotNet.Expressions.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Expressions.Ast.Conversion
{
    /// <summary>
    /// This visitor cleans up and adds any calls where required.
    /// It also removed the root unknown name from any references.
    /// </summary>
    internal class CleanupVisitor : ExpressionAstVisitor<Node>
    {
        private readonly string _rootUnknown;
        private readonly ILogger _logger;
        private string iteratorName;
        public CleanupVisitor(string rootUnknown, ILogger logger)
        {
            _rootUnknown = rootUnknown;
            _logger = logger;
        }

        private bool RemoveRootUnknown(Reference reference)
        {
            if (reference.References.Count >= 2 &&
                $"{reference.References[0].Value}.{reference.References[1].Value}" == _rootUnknown)
            {
                if (reference.References.Count < 3)
                {
                    throw new InvalidOperationException("An unknown reference must have a subobject");
                }
                if(reference.References[2] is VariableReference variableReference &&
                    variableReference.IsIterator)
                {
                    if (iteratorName == null)
                    {
                        iteratorName = variableReference.Value;
                    }
                    else if (iteratorName != variableReference.Value)
                    {
                        throw new InvalidOperationException("Cannot handle self joins");
                    }
                    reference.References.RemoveRange(0, 3);
                }
                else
                {
                    reference.References.RemoveRange(0, 2);
                }

                return true;
            }
            return false;
        }

        public override Node VisitQueries(Queries queries)
        {
            for (int i = 0; i < queries.OrQueries.Count; i++)
            {
                try
                {
                    Visit(queries.OrQueries[i]);
                }
                catch(Exception e)
                {
                    _logger.LogWarning(e, "Failure in an OR query, removing it from the final query.");
                    //Remove any or query that caused an exception.
                    queries.OrQueries.RemoveAt(i);
                    i--;
                }
            }
            return base.VisitQueries(queries);
        }

        public override Node VisitQuery(Query query)
        {
            for (int i = 0; i < query.AndExpressions.Count; i++)
            {
                query.AndExpressions[i] = Visit(query.AndExpressions[i]) as BooleanExpression;
            }
            //Reset the iterator name
            iteratorName = null;
            return query;
        }

        private bool ContainsIterator(Reference reference)
        {
            return reference.References.Any(x => x is VariableReference it && it.IsIterator);
        }

        private Node ReplaceWithAny(BooleanComparisonExpression booleanComparisonExpression)
        {
            if (booleanComparisonExpression.Left is Reference reference)
            {
                for (int i = 0; i < reference.References.Count; i++)
                {
                    var r = reference.References[i];
                    if (r is VariableReference variableReference && variableReference.IsIterator)
                    {
                        var property = new Reference()
                        {
                            References = reference.References.Take(i).ToList()
                        };

                        List<ReferenceValue> newReferences = new List<ReferenceValue>();
                        newReferences.Add(new ParameterReference()
                        {
                            Value = variableReference.Value
                        });
                        newReferences.AddRange(reference.References.Skip(i + 1));
                        var newReference = new Reference()
                        {
                            References = newReferences
                        };

                        var anyCall = new AnyCall()
                        {
                            ParameterName = variableReference.Value,
                            Property = property,
                            AndExpressions = new List<BooleanExpression>()
                            {
                                new BooleanComparisonExpression()
                                {
                                    Type = booleanComparisonExpression.Type,
                                    Right = booleanComparisonExpression.Right,
                                    Left = newReference
                                }
                            }
                        };
                        Visit(anyCall);
                        return anyCall;
                    }
                }
            }
            if (booleanComparisonExpression.Right is Reference rightReference)
            {
                for (int i = 0; i < rightReference.References.Count; i++)
                {
                    var r = rightReference.References[i];
                    if (r is VariableReference variableReference && variableReference.IsIterator)
                    {
                        var property = new Reference()
                        {
                            References = rightReference.References.Take(i).ToList()
                        };

                        List<ReferenceValue> newReferences = new List<ReferenceValue>();
                        newReferences.Add(new ParameterReference()
                        {
                            Value = variableReference.Value
                        });
                        newReferences.AddRange(rightReference.References.Skip(i + 1));
                        var newReference = new Reference()
                        {
                            References = newReferences
                        };

                        var anyCall = new AnyCall()
                        {
                            ParameterName = variableReference.Value,
                            Property = property,
                            AndExpressions = new List<BooleanExpression>()
                            {
                                new BooleanComparisonExpression()
                                {
                                    Type = booleanComparisonExpression.Type,
                                    Right = newReference,
                                    Left = booleanComparisonExpression.Left
                                }
                            }
                        };
                        Visit(anyCall);
                        return anyCall;
                    }
                }
            }
            return booleanComparisonExpression;
        }

        public override Node VisitBooleanComparisonExpression(BooleanComparisonExpression booleanComparisonExpression)
        {
            Node output = booleanComparisonExpression;
            if (booleanComparisonExpression.Left is Reference reference)
            {
                RemoveRootUnknown(reference);
                if (ContainsIterator(reference))
                {
                    //Replace the iterator with any
                    output = ReplaceWithAny(booleanComparisonExpression);
                }
            }
            if (booleanComparisonExpression.Right is Reference reference2)
            {
                RemoveRootUnknown(reference2);

                //Check if the reference contains an iterator
                if (ContainsIterator(reference2))
                {
                    //Replace the iterator with any
                    output = ReplaceWithAny(booleanComparisonExpression);
                }
            }
            return output;
        }
    }
}
