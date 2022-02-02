using NUnit.Framework;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Core.Tests.Ast.Models
{
    public class AstExpressionTests
    {
        [Test]
        public void TestEquals()
        {
            AstExpression astExpression = new AstExpression()
            {
                Index = 1,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test" } }
                }
            };

            AstExpression astExpressionCopy = new AstExpression()
            {
                Index = 1,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test" } }
                }
            };

            AstExpression differentIndex = new AstExpression()
            {
                Index = 2,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test" } }
                }
            };

            AstExpression differentTerms = new AstExpression()
            {
                Index = 1,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test2" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test" } }
                }
            };

            AstExpression differentWith = new AstExpression()
            {
                Index = 1,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test2" } }
                }
            };

            Assert.True(astExpression.Equals(astExpressionCopy));
            Assert.False(astExpression.Equals(differentIndex));
            Assert.False(astExpression.Equals(differentTerms));
            Assert.False(astExpression.Equals(differentWith));
        }

        [Test]
        public void TestGetHasCode()
        {
            AstExpression astExpression = new AstExpression()
            {
                Index = 1,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test" } }
                }
            };

            AstExpression astExpressionCopy = new AstExpression()
            {
                Index = 1,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test" } }
                }
            };

            AstExpression differentIndex = new AstExpression()
            {
                Index = 2,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test" } }
                }
            };

            AstExpression differentTerms = new AstExpression()
            {
                Index = 1,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test2" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test" } }
                }
            };

            AstExpression differentWith = new AstExpression()
            {
                Index = 1,
                Terms = new List<AstTerm>()
                {
                    new AstTermString() { Value = "test" }
                },
                With = new List<AstWith>()
                {
                    new AstWith(){ Value = new AstTermString() { Value = "test2" } }
                }
            };

            Assert.AreEqual(astExpression.GetHashCode(), astExpressionCopy.GetHashCode());
            Assert.AreNotEqual(astExpression.GetHashCode(), differentIndex.GetHashCode());
            Assert.AreNotEqual(astExpression.GetHashCode(), differentTerms.GetHashCode());
            Assert.AreNotEqual(astExpression.GetHashCode(), differentWith.GetHashCode());
        }
    }
}
