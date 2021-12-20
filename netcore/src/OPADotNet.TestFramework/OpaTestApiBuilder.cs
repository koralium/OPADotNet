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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded;
using OPADotNet.Models;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Net;

namespace OPADotNet.TestFramework
{
    /// <summary>
    /// Builder that creates an OPA test server to be used for unit testing
    /// </summary>
    public class OpaTestApiBuilder
    {
        private Dictionary<string, string> _policies = new Dictionary<string, string>();
        private Dictionary<string, object> _data = new Dictionary<string, object>();
        /// <summary>
        /// Add a policy that will be usable in the test API
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="policyData"></param>
        /// <returns></returns>
        public OpaTestApiBuilder AddPolicy(string fileName, string policyData)
        {
            _policies.Add(fileName, policyData);
            return this;
        }

        /// <summary>
        /// Add data that will be available in the test api
        /// </summary>
        /// <param name="path">Example: /roles</param>
        /// <param name="input">Object of the data that should be inserted</param>
        /// <returns></returns>
        public OpaTestApiBuilder AddData(string path, object input)
        {
            _data.Add(path, input);
            return this;
        }

        private OpaClientEmbedded CreateEmbeddedClient()
        {
            var client = new OpaClientEmbedded();
            OpaStore store = client.OpaStore;
            var txn = store.NewTransaction(true);
            foreach(var policy in _policies)
            {
                txn.UpsertPolicy(policy.Key, policy.Value);
            }

            foreach(var data in _data)
            {
                txn.Write(data.Key, data.Value);
            }
            txn.Commit();
            return client;
        }

        private List<Policy> GetPolicies()
        {
            OpaCompiler opaCompiler = new OpaCompiler(_policies);

            var astPolicies = opaCompiler.GetPolicies();
            List<Policy> output = new List<Policy>();
            foreach (var policy in _policies)
            {
                if (astPolicies.TryGetValue(policy.Key, out var astPolicy))
                {
                    output.Add(new Policy(policy.Key, policy.Value, astPolicy));
                }
            }
            return output;
        }

        /// <summary>
        /// Creates a rest client that will be calling the test API.
        /// This option does not require a server to be hosted.
        /// </summary>
        /// <returns></returns>
        public RestOpaClient BuildRestApiClient()
        {
            return new TestOpaClientRest(CreateEmbeddedClient(), GetPolicies());
        }

        /// <summary>
        /// Run the test API as a server
        /// </summary>
        /// <param name="port">The port to run the server on.</param>
        public IHost RunServer(int port)
        {
            var host = Host.CreateDefaultBuilder()
                        .ConfigureWebHostDefaults(webBuilder =>
                        {
                            _ = webBuilder
                            .ConfigureKestrel(c =>
                            {
                                c.Listen(IPEndPoint.Parse($"0.0.0.0:{port}"), l => l.Protocols = HttpProtocols.Http1AndHttp2);
                            })
                            .UseStartup<TestApiStartup>()
                            .ConfigureServices((services) =>
                            {
                                services.AddSingleton(new TestOpaClientRest(CreateEmbeddedClient(), GetPolicies()));
                            });
                        }).Build();
            host.Start();
            return host;
        }
    }
}
