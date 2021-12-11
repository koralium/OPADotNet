using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.Tests
{
    internal class EvaluationTests
    {
        private class ReturnVal
        {
            public List<String> List { get; set; }
        }

        [Test]
        public void TestRunPreparedPartial()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
                OpaStore opaStore = opaClientEmbedded.OpaStore;
                var txn = opaStore.NewTransaction(true);
                txn.UpsertPolicy("policy", moduleData);
                txn.Commit();

                var prepared = opaClientEmbedded.PrepareEvaluation("List = data.example.violation");

                var o = (await prepared.Evaluate<ReturnVal>()).ToList();

                //o.FirstOrDefault()
            });
        }

        private static string moduleData = @"
        package example

        violation[msg] {
            time.parse_rfc3339_ns(""1990-12-31T23:59:60Z"")
            1 < 0
            
            msg := ""This is a message""
        }
        ";
    }
}
