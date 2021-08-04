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
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OPADotNet.Embedded.Discovery;
using OPADotNet.Embedded.sync;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace OPADotNet.Embedded.Tests
{
    public class DiscoveryTests
    {
        WireMockServer _server;
        [SetUp]
        public void StartMockServer()
        {
            _server = WireMockServer.Start();

            var fileBytes = File.ReadAllBytes("test.tar.gz");
            _server.Given(Request.Create().WithPath("/testing/test").WithHeader("x-ms-version", "2020-04-08").UsingGet())
                .RespondWith(
                    Response.Create().WithStatusCode(200)
                    .WithBody(fileBytes).WithHeader("Content-Type", "application/octet-stream")
                );
        }

        [TearDown]
        public void ShutdownServer()
        {
            _server.Stop();
        }

        [Test]
        public async Task TestDiscovery()
        {
            string configResult = @"
            {
              ""services"": {
                ""test"": {
                  ""url"": """ + _server.Urls[0] + @""",
                  ""headers"": {
                    ""x-ms-version"": ""2020-04-08""
                  }
                }
              },
              ""bundles"": {
                ""main"": {
                  ""service"": ""test"",
                  ""resource"": ""testing/test"",
                  ""polling"": {
                    ""min_delay_seconds"": 30
                  }
                }
              }
            }
            ";

            ServiceCollection services = new ServiceCollection();
            services.AddLogging();

            var opaClientEmbedded = new OpaClientEmbedded();
            services.AddSingleton(opaClientEmbedded);

            var localSyncOptions = new LocalSyncOptions()
                .AddData("/", configResult);

            SyncServiceHolder syncServiceHolder = new SyncServiceHolderObject(new LocalSync(localSyncOptions));

            services.AddSingleton(new DiscoveryOptions(syncServiceHolder, "data"));
            services.AddSingleton(new SyncOptions(new List<SyncServiceHolder>()));

            DiscoveryHandler discoveryHandler = new DiscoveryHandler(new List<SyncPolicyDescriptor>()
            {
                new SyncPolicyDescriptor()
                {
                    PolicyName = "example",
                    Unknown = "reports"
                }
            }, services.BuildServiceProvider());

            await discoveryHandler.Start();

            var readTxn = opaClientEmbedded.OpaStore.NewTransaction(false);
            var rolesData = readTxn.Read("/roles/inner");
            readTxn.Commit();

            Assert.AreEqual("{\"identity\":\"test\"}", rolesData);
        }
    }
}
