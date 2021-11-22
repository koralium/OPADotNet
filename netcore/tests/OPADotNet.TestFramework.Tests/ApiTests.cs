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
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using OPADotNet.Ast.Models;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OPADotNet.TestFramework.Tests
{
    public class ApiTests
    {
        private static string moduleText = @"
                package testpolicy

                allow {
	                input.subject
                    data.reports[_].test = true
                }
            ";

        private IHost _server;

        [OneTimeSetUp]
        public void Setup()
        {
            _server = new OpaTestApiBuilder()
                .AddPolicy("test", moduleText)
                .AddData("/roles", new
                {
                    name = "test"
                })
                .AddData("/user", new
                {
                    name = "test",
                    role = "test"
                })
                .RunServer(5010);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            if (_server != null)
            {
                await _server.StopAsync();
                _server.Dispose();
            }
        }

        private class RunQueryResponse
        {
            [JsonPropertyName("key")]
            public string Key { get; set; }

            [JsonPropertyName("value")]
            public string Value { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is RunQueryResponse other)
                {
                    return Key.Equals(other.Key) &&
                        Value.Equals(other.Value);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, Value);
            }

            public override string ToString()
            {
                return $"Key = {Key}, Value = {Value}";
            }
        }

        [Test]
        public async Task TestRunQuery()
        {
            RestOpaClient restOpaClient = new RestOpaClient("http://127.0.0.1:5010");
            var rolesData = await restOpaClient.PrepareEvaluation("data.roles[key] = value").Evaluate<object>();
            var stringContent = rolesData.First().ToString();
            Assert.AreEqual("{\"key\":\"name\",\"value\":\"test\"}", stringContent);

            var userData = await restOpaClient.PrepareEvaluation("data.user[key] = value").Evaluate<RunQueryResponse>();
            //stringContent = JsonSerializer.Serialize(userData);
            List<RunQueryResponse> expeced = new List<RunQueryResponse>()
           {
               new RunQueryResponse()
               {
                   Key = "name",
                   Value = "test"
               },
               new RunQueryResponse()
               {
                   Key = "role",
                   Value = "test"
               }
           };
            Assert.That(userData, Is.EquivalentTo(expeced));
        }

        [Test]
        public async Task TestGetData()
        {
            RestOpaClient restOpaClient = new RestOpaClient("http://127.0.0.1:5010");
            var rolesData = await restOpaClient.GetData<object>("/roles");
            var stringContent = rolesData.ToString();
            Assert.AreEqual("{\"name\":\"test\"}", stringContent);
        }

        [Test]
        public async Task TestCompile()
        {
            RestOpaClient restOpaClient = new RestOpaClient("http://127.0.0.1:5010");
            var prepared = restOpaClient.PreparePartial("data.testpolicy.allow == true");
            var partialResult = await prepared.Partial(new
            {
                subject = new
                {
                    name = "test"
                }
            }, new List<string>()
            {
                "data.reports"
            });

            var expected = new AstQueries()
            {
                Queries = new List<AstBody>()
                {
                    new AstBody()
                    {
                        Expressions = new List<AstExpression>()
                        {
                            new AstExpression()
                            {
                                Index = 0,
                                Terms = new List<AstTerm>()
                                {
                                    new AstTermRef()
                                    {
                                        Value = new List<AstTerm>()
                                        {
                                            new AstTermVar()
                                            {
                                                Value = "eq"
                                            }
                                        }
                                    },
                                    new AstTermRef()
                                    {
                                        Value = new List<AstTerm>()
                                        {
                                            new AstTermVar()
                                            {
                                                Value = "data"
                                            },
                                            new AstTermString()
                                            {
                                                Value = "reports"
                                            },
                                            new AstTermVar()
                                            {
                                                Value = "$01"
                                            },
                                            new AstTermString()
                                            {
                                                Value = "test"
                                            }
                                        }
                                    },
                                    new AstTermBoolean()
                                    {
                                        Value = true
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Assert.AreEqual(expected, partialResult);
        }

        [Test]
        public async Task TestGetPolicies()
        {
            RestOpaClient restOpaClient = new RestOpaClient("http://127.0.0.1:5010");
            var policies = await restOpaClient.GetPolicies();

            Assert.NotNull(policies);
            Assert.AreEqual(1, policies.Count);
            Assert.AreEqual("test", policies[0].Id);
            Assert.AreEqual(moduleText, policies[0].Raw);

            var astPolicy = new AstPolicy()
            {
                Package = new AstPolicyPackage()
                {
                    Path = new List<AstTerm>()
                    {
                        new AstTermVar()
                        {
                            Value = "data"
                        },
                        new AstTermString()
                        {
                            Value = "testpolicy"
                        }
                    }
                },
                Rules = new List<AstPolicyRule>()
                {
                    new AstPolicyRule()
                    {
                        Head = new AstRuleHead()
                        {
                            Name = "allow",
                            Value = new AstTermBoolean()
                            {
                                Value = true
                            }
                        },
                        Body = new AstBody()
                        {
                            Expressions = new List<AstExpression>()
                            {
                                new AstExpression()
                                {
                                    Index = 0,
                                    Terms = new List<AstTerm>()
                                    {
                                        new AstTermRef()
                                        {
                                            Value = new List<AstTerm>()
                                            {
                                                new AstTermVar()
                                                {
                                                    Value = "input"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "subject"
                                                }
                                            }
                                        }
                                    }
                                },
                                new AstExpression()
                                {
                                    Index = 1,
                                    Terms = new List<AstTerm>()
                                    {
                                        new AstTermRef()
                                        {
                                            Value = new List<AstTerm>()
                                            {
                                                new AstTermVar()
                                                {
                                                    Value = "eq"
                                                }
                                            }
                                        },
                                        new AstTermRef()
                                        {
                                            Value = new List<AstTerm>()
                                            {
                                                new AstTermVar()
                                                {
                                                    Value = "data"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "reports"
                                                },
                                                new AstTermVar()
                                                {
                                                    Value = "$0"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "test"
                                                }
                                            }
                                        },
                                        new AstTermBoolean()
                                        {
                                            Value = true
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Assert.AreEqual(astPolicy, policies[0].Ast);
        }
    }
}