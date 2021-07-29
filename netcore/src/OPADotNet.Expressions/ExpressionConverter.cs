using OPADotNet.Ast.Models;
using OPADotNet.Expressions.Ast;
using OPADotNet.Expressions.Ast.Conversion;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OPADotNet.Expressions
{
    public class ExpressionConverter
    {
        public async Task<Expression<Func<object, bool>>> ToExpression(AstQueries partialQueries, string unknown, Type queryType)
        {
            var convertor = new PartialToAstVisitor();
            var ast = convertor.Convert(partialQueries);
            CleanupVisitor cleanupVisitor = new CleanupVisitor(unknown);
            cleanupVisitor.Visit(ast);

            ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
            var convertedParameter = Expression.Convert(parameterExpression, queryType);

            AstToExpressionVisitor astToExpressionVisitor = new AstToExpressionVisitor(convertedParameter, queryType);
            var expression = astToExpressionVisitor.Visit(ast);
            return Expression.Lambda(expression, parameterExpression) as Expression<Func<object, bool>>;
        }

        public async Task<Expression> ToExpression(AstQueries partialQueries, string unknown, ParameterExpression parameterExpression)
        {
            var convertor = new PartialToAstVisitor();
            var ast = convertor.Convert(partialQueries);
            CleanupVisitor cleanupVisitor = new CleanupVisitor(unknown);
            cleanupVisitor.Visit(ast);
            AstToExpressionVisitor astToExpressionVisitor = new AstToExpressionVisitor(parameterExpression, parameterExpression.Type);
            return astToExpressionVisitor.Visit(ast);
        }

        //public async Task<Expression<Func<TType, bool>>> ToExpression<TType>(IPreparedPartial preparedPartial, object input, string unknown)
        //{
        //    var partialQueries = await preparedPartial.Partial(input, new List<string>() { unknown });

        //    if (partialQueries == null)
        //    {
        //        //Return function that just returns false here
        //    }

        //    var convertor = new PartialToAstVisitor();
        //    var ast = convertor.Convert(partialQueries);
        //    CleanupVisitor cleanupVisitor = new CleanupVisitor(unknown);
        //    cleanupVisitor.Visit(ast);
        //    AstToExpressionVisitor astToExpressionVisitor = new AstToExpressionVisitor(typeof(TType));
        //    return astToExpressionVisitor.Visit(ast) as Expression<Func<TType, bool>>;
        //}
    }
}
