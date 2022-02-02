using Moq;
using NUnit.Framework;
using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Core.Tests.Ast
{
    internal class VisitorTests
    {
        [Test]
        public void TestVisitWith()
        {
            AstVisitor<object> visitor = new AstVisitor<object>();

            Mock<AstTerm> target = new Mock<AstTerm>();
            Mock<AstTerm> value = new Mock<AstTerm>();

            AstWith with = new AstWith()
            {
                Target = target.Object,
                Value = value.Object
            };

            visitor.Visit(with);

            target.Verify(x => x.Accept(visitor), Times.Once);
            value.Verify(x => x.Accept(visitor), Times.Once);
        }

        [Test]
        public void TestVisitExpression()
        {
            AstVisitor<object> visitor = new AstVisitor<object>();

            Mock<AstTerm> target1 = new Mock<AstTerm>();
            Mock<AstWith> target2 = new Mock<AstWith>();

            AstExpression expr = new AstExpression()
            {
                Index = 0,
                Terms = new List<AstTerm>() { target1.Object },
                With = new List<AstWith>() { target2.Object }
            };

            visitor.Visit(expr);

            target1.Verify(x => x.Accept(visitor), Times.Once);
            target2.Verify(x => x.Accept(visitor), Times.Once);
        }
    }
}
