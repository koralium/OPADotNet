using BenchmarkDotNet.Attributes;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded;
using OPADotNet.Embedded.Internal;
using OPADotNet.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Benchmarks
{
    
    public class NativePartialEval
    {
        private IPreparedPartial preparedPartial;
        private AstQueries astQueries;
        [GlobalSetup]
        public async Task Setup()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            var txn = opaClientEmbedded.OpaStore.NewTransaction(true);
            txn.UpsertPolicy("test", @"
                package test

                allow {
                    some i
                    data.t[i].list[_] = ""test1""
                    data.t[i].list[_] = ""test2""
                }");

            txn.Commit();
            preparedPartial = opaClientEmbedded.PreparePartial("data.test.allow == true");
            astQueries = (await preparedPartial.Partial(new
            {
                subject = new
                {
                    name = "test"
                }
            }, new List<string>() { "data.t" })).Result;
        }

        [Benchmark]
        public async Task PreparedPartial()
        {
            await preparedPartial.Partial(new
            {
                subject = new
                {
                    name = "test"
                }
            }, new List<string>() { "data.t" });
        }

        private class TestModel
        {
            public List<string> List { get; set; }
        }

        [Benchmark]
        public async Task ToExpression()
        {
            await new ExpressionConverter(null).ToExpression(astQueries, "data.t", typeof(TestModel));
        }
    }
}
