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

            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            SyncContext syncContext = new SyncContext(new List<SyncPolicyDescriptor>()
            {
                new SyncPolicyDescriptor("example", "reports")
            }, opaClientEmbedded, new List<Type>(), null);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await tarGzSync.FullLoad(syncContext, cancellationTokenSource.Token);

            var readTxn = opaClientEmbedded.OpaStore.NewTransaction(false);
            var rolesData = readTxn.Read("/roles/inner");
            readTxn.Commit();

            Assert.AreEqual("{\"identity\":\"test\"}", rolesData);
        }
    }
}
