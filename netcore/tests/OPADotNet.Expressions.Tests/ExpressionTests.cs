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
using NUnit.Framework;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded;
using OPADotNet.Expressions.Ast;
using OPADotNet.Expressions.Ast.Conversion;
using OPADotNet.Expressions.Ast.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OPADotNet.Expressions.Tests
{
    public class ExpressionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        private async Task<AstQueries> Execute(string policyText, string query, object input, string unknown)
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();

            var txn = opaClientEmbedded.OpaStore.NewTransaction(true);
            txn.UpsertPolicy(Guid.NewGuid().ToString(), policyText);
            txn.Commit();
            var preparedPartial = opaClientEmbedded.PreparePartial(query);
            var astQueries = await preparedPartial.Partial(input, new List<string>()
            {
                unknown
            });
            return astQueries;
        }

        private async Task<Func<object, bool>> GetFunc(string policyText, string query, object input, string unknown, Type t)
        {
            var ast = await Execute(policyText, query, input, unknown);
            return (await new ExpressionConverter(null).ToExpression(ast, unknown, t)).Compile();
        }

        private class TestModel
        {
            public List<string> List { get; set; }
        }

        [Test]
        public async Task TestMergeAnyCalls()
        {
            var testModel = new TestModel()
            {
                List = new List<string>()
                {
                    "test1",
                    "test2"
                }
            };

            string policy = @"
                package test

                allow {
                    some i, j
                    data.t[i].list[j] = ""test1""
                    data.t[i].list[j] = ""test2""
                }
                ";

            var result = await GetFunc(policy, "data.test.allow == true", new { }, "data.t", typeof(TestModel));

            Assert.False(result(testModel));

            policy = @"
                package test

                allow {
                    some i
                    data.t[i].list[_] = ""test1""
                    data.t[i].list[_] = ""test2""
                }
                ";

            result = await GetFunc(policy, "data.test.allow == true", new { }, "data.t", typeof(TestModel));

            Assert.True(result(testModel));
        }
    }
}