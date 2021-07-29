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

        public AuthorizeQueryableHolder(Type entityType, ParameterExpression parameterExpression)
        {
            EntityType = entityType;
            ParameterExpression = parameterExpression;
        }
    }
}
