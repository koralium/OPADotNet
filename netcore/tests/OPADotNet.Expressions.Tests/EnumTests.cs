using NUnit.Framework;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Expressions.Tests
{
    public class EnumTests
    {
        private async Task<AstQueries> Execute(string policyText, string query, object input, string unknown)
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();

            var txn = opaClientEmbedded.OpaStore.NewTransaction(true);
            txn.UpsertPolicy(Guid.NewGuid().ToString(), policyText);
            txn.Commit();
            var preparedPartial = opaClientEmbedded.PreparePartial(query);
            var partialResult = await preparedPartial.Partial(input, new List<string>()
            {
                unknown
            });
            return partialResult.Result;
        }

        private async Task<Func<object, bool>> GetFunc(string policyText, string query, object input, string unknown, Type t)
        {
            var ast = await Execute(policyText, query, input, unknown);
            return (await new ExpressionConverter(null).ToExpression(ast, unknown, t)).Compile();
        }

        private enum Enum
        {
            Value1,
            Value2
        }

        private class TestModel
        {
            public Enum EnumValue { get; set; }
        }

        [Test]
        public async Task TestEnum()
        {
            string policy = @"
                package test

                allow {
                    data.t.enumValue = ""Value1""
                }
                ";

            var result = await GetFunc(policy, "data.test.allow == true", new { }, "data.t", typeof(TestModel));

            var model = new TestModel()
            {
                EnumValue = Enum.Value1,
            };

            var actual = result(model);
            Assert.AreEqual(true, actual);
        }
    }
}
