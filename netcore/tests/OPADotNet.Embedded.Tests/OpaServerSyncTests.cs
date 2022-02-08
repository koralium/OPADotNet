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
using Microsoft.Extensions.DependencyInjection;
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

            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            var provider = services.BuildServiceProvider();

            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            SyncContext syncContext = new SyncContext(new List<SyncPolicyDescriptor>()
            {
                new SyncPolicyDescriptor("example", "reports")
            }, opaClientEmbedded, new List<Type>(), provider);

            

            OpaServerSync opaServerSync = new OpaServerSync(new OpaServerSyncOptions()
            {
                OpaServerUrl = "http://127.0.0.1:5020"
            }, null);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await opaServerSync.Initialize(provider);
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

            Assert.That(partialResult.Result.Queries.Count == 1 && partialResult.Result.Queries[0].Expressions.Count == 0);

            partialResult = await prepared.Partial(new
            {
                identity = "bob",
                test = "abc"
            }, new List<string>());

            Assert.That(partialResult.Result.Queries == null);
        }
    }
}
