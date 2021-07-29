using OPADotNet.Expressions.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Expressions.Ast
{
    internal class ExpressionAstVisitor<T>
    {
        private static readonly List<T> emptyList = new List<T>();

        public virtual T Visit(Node node)
        {
            if (node == null)
                return default;

            return node.Accept(this);
        }

        public virtual IList<T> Visit(IEnumerable<Node> nodes)
        {
            if (nodes == null)
                return emptyList;

            List<T> output = new List<T>();
            foreach (var node in nodes)
            {
                output.Add(Visit(node));
            }
            return output;
        }

        public virtual T VisitAnyCall(AnyCall anyCall)
        {
            Visit(anyCall.Property);
            Visit(anyCall.AndExpressions);
            return default;
        }

        public virtual T VisitBooleanComparisonExpression(BooleanComparisonExpression booleanComparisonExpression)
        {
            Visit(booleanComparisonExpression.Left);
            Visit(booleanComparisonExpression.Right);
            return default;
        }

        public virtual T VisitNumericLiteral(NumericLiteral numericLiteral)
        {
            return default;
        }

        public virtual T VisitQueries(Queries queries)
        {
            Visit(queries.OrQueries);
            return default;
        }

        public virtual T VisitQuery(Query query)
        {
            Visit(query.AndExpressions);
            return default;
        }

        public virtual T VisitReference(Reference reference)
        {
            Visit(reference.References);
            return default;
        }

        public virtual T VisitStringLiteral(StringLiteral stringLiteral)
        {
            return default;
        }

        public virtual T VisitStringReference(StringReference stringReference)
        {
            return default;
        }

        public virtual T VisitVariableReference(VariableReference variableReference)
        {
            return default;
        }

        public virtual T VisitParameterReference(ParameterReference parameterReference)
        {
            return default;
        }

        public virtual T VisitBoolLiteral(BoolLiteral boolLiteral)
        {
            return default;
        }
    }
}
