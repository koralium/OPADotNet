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
using OPADotNet.Expressions.Ast.Conversion;
using OPADotNet.Expressions.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Expressions.Ast
{
    internal class PartialToAstVisitor : AstVisitor<Node>
    {
        private static OperatorConverter operatorConverter = new OperatorConverter();
        private static ScalarExpressionConverter scalarExpressionConverter = new ScalarExpressionConverter();

        public Queries Convert(AstQueries partialQueries)
        {
            return Visit(partialQueries) as Queries;
        }

        public override Node VisitQueries(AstQueries partialQueries)
        {
            List<Query> queries = new List<Query>(partialQueries.Queries.Count);

            for (int i = 0; i < partialQueries.Queries.Count; i++)
            {
                var query = Visit(partialQueries.Queries[i]) as Query;

                if (query == null)
                {
                    throw new InvalidOperationException("Could not find a query.");
                }

                queries.Add(query);
            }
            return new Queries()
            {
                OrQueries = queries
            };
        }

        public override Node VisitBody(AstBody partialBody)
        {
            List<BooleanExpression> booleanExpressions = new List<BooleanExpression>(partialBody.Expressions.Count);

            for (int i = 0; i < partialBody.Expressions.Count; i++)
            {
                var expression = Visit(partialBody.Expressions[i]) as BooleanExpression;

                if (expression == null)
                {
                    throw new InvalidOperationException("Could not find a boolean expression in expression number: " + i);
                }

                booleanExpressions.Add(expression);
            }

            return new Query()
            {
                AndExpressions = booleanExpressions
            };
        }

        public override Node VisitExpression(AstExpression partialExpression)
        {
            if (partialExpression.Terms.Count != 3)
            {
                throw new InvalidOperationException("Expressions can only be created if the expression terms are of length 3.");
            }
            var op = operatorConverter.Visit(partialExpression.Terms[0]);

            if (op == null)
            {
                throw new InvalidOperationException("Could not find the comparison operator.");
            }

            var left = scalarExpressionConverter.Visit(partialExpression.Terms[1]);
            var right = scalarExpressionConverter.Visit(partialExpression.Terms[2]);

            return new BooleanComparisonExpression()
            {
                Left = left,
                Right = right,
                Type = op.Value
            };
        }
    }
}
