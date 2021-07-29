using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using OPADotNet.Ast.Models;
using OPADotNet.RestAPI;
using System.Collections.Generic;
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
                .RunServer(5010);
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _server.StopAsync();
            _server.Dispose();
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