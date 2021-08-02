using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OPADotNet.Embedded.Discovery;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    public class DiscoveryTests
    {
        [Test]
        public async Task TestDiscovery()
        {
            string configResult = @"
            {
              ""bundles"": {
                ""main"": {
                  ""service"": ""acmecorp"",
                  ""resource"": ""acmecorp/httpauthz""
                }
              },
              ""default_decision"": ""acmecorp/httpauthz/allow""
            }
            ";
            ServiceCollection services = new ServiceCollection();

            var localSyncOptions = new LocalSyncOptions()
                .AddData("/", configResult);

            SyncServiceHolder syncServiceHolder = new SyncServiceHolderObject(new LocalSync(localSyncOptions));

            services.AddSingleton(new DiscoveryOptions(syncServiceHolder, "data"));

            DiscoveryHandler discoveryHandler = new DiscoveryHandler(new List<sync.SyncPolicyDescriptor>(), services.BuildServiceProvider());

            await discoveryHandler.Start();
        }
    }
}
