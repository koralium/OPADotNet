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
using OPADotNet.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace OPADotNet.AspNetCore
{
    internal class AuthorizeQueryableHolder
    {
        public Type EntityType { get; }
        public ParameterExpression ParameterExpression { get; }
        public ExpressionConversionOptions Options { get; }

        private readonly List<Expression> expressions = new List<Expression>();

        public void AddFilter(Expression expression)
        {
            expressions.Add(expression);
        }

        public LambdaExpression GetLambdaExpression()
        {
            if (expressions.Count == 0)
            {
                return Expression.Lambda(Expression.Constant(false), ParameterExpression);
            }
            var expr = expressions.First();
            for (int i = 1; i < expressions.Count; i++)
            {
                expr = Expression.AndAlso(expr, expressions[i]);
            }
            return Expression.Lambda(expr, ParameterExpression);
        }

        public AuthorizeQueryableHolder(Type entityType, ParameterExpression parameterExpression, ExpressionConversionOptions options)
        {
            EntityType = entityType;
            ParameterExpression = parameterExpression;
            Options = options;
        }
    }
}
