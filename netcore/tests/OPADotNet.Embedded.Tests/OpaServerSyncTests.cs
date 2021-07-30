using NUnit.Framework;
using OPADotNet.Embedded.sync;
using OPADotNet.Embedded.Sync;
using OPADotNet.TestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    public class OpaServerSyncTests
    {
        private static string moduleData = @"
package example

default allow = false

allow {
  data.roles.identity = input.identity
  data.roles.test = input.test
}
";

        [Test]
        public async Task TestSyncPolicyAndData()
        {
            var opaClient = new OpaTestApiBuilder()
                .AddPolicy("test", moduleData)
                .AddData("/roles", new
                {
                    identity = "test",
                    test = "abc"
                })
                .RunServer(5020);

            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            SyncContext syncContext = new SyncContext(new List<SyncPolicyDescriptor>()
            {
                new SyncPolicyDescriptor()
                {
                    PolicyName = "example",
                    Unknown = "reports"
                }
            }, opaClientEmbedded);

            OpaServerSync opaServerSync = new OpaServerSync(new OpaServerSyncOptions()
            {
                OpaServerUrl = "http://127.0.0.1:5020"
            }, null);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await opaServerSync.FullLoad(syncContext, cancellationTokenSource.Token);

            var readTxn = opaClientEmbedded.OpaStore.NewTransaction(false);
            var rolesData = readTxn.Read("/roles");
            readTxn.Commit();

            Assert.AreEqual("{\"identity\":\"test\",\"test\":\"abc\"}", rolesData);

            var prepared = opaClientEmbedded.PreparePartial("data.example.allow == true");
            var partialResult = await prepared.Partial(new
            {
                identity = "test",
                test = "abc"
            }, new List<string>());

            Assert.That(partialResult.Queries.Count == 1 && partialResult.Queries[0].Expressions.Count == 0);

            partialResult = await prepared.Partial(new
            {
                identity = "bob",
                test = "abc"
            }, new List<string>());

            Assert.That(partialResult.Queries == null);
        }
    }
}
