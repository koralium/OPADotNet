using NUnit.Framework;
using OPADotNet.Ast.Models;
using OPADotNet.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    internal class GetPoliciesTest
    {
        [Test]
        public async Task TestGetSinglePolicy()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            var txn = opaClientEmbedded.OpaStore.NewTransaction(true);
            var rawPolicy = @"
            package test

            allow = true
            ";
            txn.UpsertPolicy("pol1", rawPolicy);
            txn.Commit();

            var policies = await opaClientEmbedded.GetPolicies();

            var expected = new List<Policy>()
            {
                new Policy(
                    "pol1.rego",
                    rawPolicy,
                    new AstPolicy()
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
                                    Value = "test"
                                }
                            }
                        },
                        Rules = new List<AstPolicyRule>()
                        {
                            new AstPolicyRule()
                            {
                                Body = new AstBody()
                                {
                                    Expressions = new List<AstExpression>()
                                    {
                                        new AstExpression()
                                        {
                                            Terms = new List<AstTerm>()
                                            {
                                                new AstTermBoolean(){ Value = true}
                                            }
                                        }
                                    }
                                },
                                Head = new AstRuleHead()
                                {
                                    Name = "allow",
                                    Value = new AstTermBoolean()
                                    {
                                        Value = true
                                    }
                                }
                            }
                        }
                    }
                )
            };

            Assert.AreEqual(expected, policies);
        }

        [Test]
        public void TestGetPolicyWithStatementDoNotThrow()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            var txn = opaClientEmbedded.OpaStore.NewTransaction(true);
            var rawPolicy = @"
            package test

            allow = true

            test_case {
                allow with input as {
                    ""test"": ""t1""
                }
            }
            ";
            txn.UpsertPolicy("pol1", rawPolicy);
            txn.Commit();

            Assert.DoesNotThrowAsync(async () =>
            {
                var policies = await opaClientEmbedded.GetPolicies();
            });
        }
    }
}
