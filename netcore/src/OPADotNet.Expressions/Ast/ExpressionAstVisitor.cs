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

        public virtual T VisitNullLiteral(NullLiteral nullLiteral)
        {
            return default;
        }
    }
}
