using NUnit.Framework;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Core.Tests.Ast
{
    public class AstWithTests
    {
        [Test]
        public void TestAstWithEquals()
        {
            var astWith = new AstWith()
            {
                Target = new AstTermString() { Value = "1" },
                Value = new AstTermString() { Value = "2" }
            };

            var astWithCopy = new AstWith()
            {
                Target = new AstTermString() { Value = "1" },
                Value = new AstTermString() { Value = "2" }
            };

            var other = new AstWith()
            {
                Target = new AstTermString() { Value = "3" },
                Value = new AstTermString() { Value = "4" }
            };

            var otherType = new object();

            Assert.True(astWith.Equals(astWithCopy));
            Assert.False(astWith.Equals(other));
            Assert.False(astWith.Equals(otherType));
        }

        [Test]
        public void TestGetHashCode()
        {
            var astWith = new AstWith()
            {
                Target = new AstTermString() { Value = "1" },
                Value = new AstTermString() { Value = "2" }
            };

            var astWithCopy = new AstWith()
            {
                Target = new AstTermString() { Value = "1" },
                Value = new AstTermString() { Value = "2" }
            };

            var other = new AstWith()
            {
                Target = new AstTermString() { Value = "3" },
                Value = new AstTermString() { Value = "4" }
            };

            Assert.AreEqual(astWith.GetHashCode(), astWithCopy.GetHashCode());
            Assert.AreNotEqual(astWith.GetHashCode(), other.GetHashCode());
        }


    }
}
