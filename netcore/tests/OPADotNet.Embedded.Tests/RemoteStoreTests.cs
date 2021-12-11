using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Tests
{
    public class RemoteStoreTests
    {
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

                var preparedPartial = opaClientEmbedded.PreparePartial("d = data.example.allow");

                var partialResult = await preparedPartial.Partial(new
                {
                    subject = new
                    {
                        version = "1.0.1",
                        chart = "elasticsearch"
                    }
                }, new List<string>()
                {
                    "data.allowed_charts",
                    "data.helm_charts"
                });
            });
        }

        private static string moduleData = @"
        package example

        allow[msg] {
          input.subject.version != data.helm_charts[input.subject.chart].version
          msg := sprintf(""Wrong version (%v)"", [data.helm_charts[input.subject.chart].version])
        }

        allow[msg] {
          v := data.helm_charts[_]
          data.allowed_charts[_].name = v.name
          msg := ""other thing""
        }
        ";
    }
}
