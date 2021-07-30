using NUnit.Framework;
using OPADotNet.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestPreparePartial()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaStore();
                var txn = opaStore.NewTransaction(true);
                txn.UpsertPolicy("policy", moduleData);
                txn.Commit();
                OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded(opaStore);
                opaClientEmbedded.PreparePartial("data.example.allow == true");
            });
        }

        [Test]
        public void TestRunPreparedPartial()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                OpaStore opaStore = new OpaStore();
                var txn = opaStore.NewTransaction(true);
                txn.UpsertPolicy("policy", moduleData);
                txn.Commit();
                OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded(opaStore);
                var preparedPartial = opaClientEmbedded.PreparePartial("data.example.allow == true");

                await preparedPartial.Partial(new
                {
                    subject = new {
                    clearance_level = 20
                    }
                }, new List<string>()
                {
                    "data.reports"
                });
            });
        }

        private static string moduleData = @"
package example

allow {
  input.subject.clearance_level >= data.reports[_].clearance_level
  input.subject.clearance_level <= data.reports[_].test_level
}

# Allow if the user is a member
allow {
  data.reports[_].members[_] = data.roles.identity
  data.reports[_].members[_] = input.subject.login
}

# Allow if the user has a business area role
# User requires a level above 20
allow {
  some i, k
  data.reports[k].test = data.reports[k].asd
  data.reports[k].businessAreaId = input.subject.roles[i].businessAreaId
  data.reports[k].teamId = input.subject.roles[i].businessAreaId
  input.subject.roles[i].level >= 20
}
";

        public class TestModel
        {
            public int clearance_level { get; set; }

            public int test_level { get; set; }

            public List<string> Members { get; set; }
        }

        [Test]
        public async Task TestPartial()
        {
            OpaCompiler opaCompiler = new OpaCompiler(new Dictionary<string, string>());
            //opaCompiler.CompileModule("example.rego", moduleData);

            OpaStore store = new OpaStore();
            var txn = store.NewTransaction(true);
            txn.Write("/roles", new
            {
                identity = "test"
            });

            txn.Commit();

            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded(store);
            var preparedPartial = opaClientEmbedded.PreparePartial("data.example.allow == true");

            ExpressionConverter expressionConverter = new ExpressionConverter();

            //var expr = await expressionConverter.ToExpression<TestModel>(preparedPartial, new
            //{
            //    subject = new {
            //        login = "test",
            //        clearance_level = 20
            //    },
            //    method = "GET"
            //}, "data.reports");

            var r = await preparedPartial.Partial(new
            {
                identity = "bob",
                method = "GET"
            }, new List<string>()
            {
                "data.reports"
            });

            Assert.Pass();
        }
    }
}