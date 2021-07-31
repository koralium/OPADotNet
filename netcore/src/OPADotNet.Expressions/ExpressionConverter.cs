using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ExpressionConverter> _logger;
        public ExpressionConverter(ILogger<ExpressionConverter> logger)
        {
            _logger = logger;
        }

        public async Task<Expression<Func<object, bool>>> ToExpression(AstQueries partialQueries, string unknown, Type queryType)
        {
            var convertor = new PartialToAstVisitor();
            var ast = convertor.Convert(partialQueries);
            CleanupVisitor cleanupVisitor = new CleanupVisitor(unknown, _logger);
            cleanupVisitor.Visit(ast);

            ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
            var convertedParameter = Expression.Convert(parameterExpression, queryType);

            AstToExpressionVisitor astToExpressionVisitor = new AstToExpressionVisitor(convertedParameter, queryType, _logger);
            var expression = astToExpressionVisitor.Visit(ast);
            return Expression.Lambda(expression, parameterExpression) as Expression<Func<object, bool>>;
        }

        public async Task<Expression> ToExpression(AstQueries partialQueries, string unknown, ParameterExpression parameterExpression)
        {
            var convertor = new PartialToAstVisitor();
            var ast = convertor.Convert(partialQueries);
            CleanupVisitor cleanupVisitor = new CleanupVisitor(unknown, _logger);
            cleanupVisitor.Visit(ast);
            AstToExpressionVisitor astToExpressionVisitor = new AstToExpressionVisitor(parameterExpression, parameterExpression.Type, _logger);
            return astToExpressionVisitor.Visit(ast);
        }
    }
}
