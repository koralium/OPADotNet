using NUnit.Framework;
using OPADotNet.Embedded.sync;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    public class TarGzTest
    {
        [Test]
        public async Task TestTarGz()
        {
            var tarGzSync = new LocalTarGzSync("test.tar.gz");

            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded(new OpaStore());
            SyncContext syncContext = new SyncContext(new List<SyncPolicyDescriptor>()
            {
                new SyncPolicyDescriptor()
                {
                    PolicyName = "example",
                    Unknown = "reports"
                }
            }, opaClientEmbedded);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await tarGzSync.FullLoad(syncContext, cancellationTokenSource.Token);

            var readTxn = opaClientEmbedded.OpaStore.NewTransaction(false);
            var rolesData = readTxn.Read("/roles/inner");
            readTxn.Commit();

            Assert.AreEqual("{\"identity\":\"test\"}", rolesData);
        }
    }
}
