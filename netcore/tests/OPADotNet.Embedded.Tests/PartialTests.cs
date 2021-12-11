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
using OPADotNet.Ast.Models;
using OPADotNet.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    public class PartialTests
    {
        [Test]
        public async Task TestPartialWithModule()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            OpaStore opaStore = opaClientEmbedded.OpaStore;
            var txn = opaStore.NewTransaction(true);
            txn.UpsertPolicy("policy", @"
            package example

            allow[msg] {
                data.reports[_].level = input.subject.clearance_level
                msg := ""message""
            }
            
            ");
            txn.Commit();

            var preparedPartial = opaClientEmbedded.PreparePartial("d = data.example.allow");

            var result = await preparedPartial.Partial(new
            {
                subject = new
                {
                    clearance_level = 20
                }
            }, new List<string>()
                {
                    "data.reports"
                });

            var expected = new AstQueries()
            {
                Modules = new List<AstPolicy>()
                {
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
                                    Value = "partial"
                                },
                                new AstTermString()
                                {
                                    Value = "example"
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
                                    Key = new AstTermString()
                                    {
                                        Value = "message"
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
                                                            Value = "eq"
                                                        }
                                                    }
                                                },
                                                new AstTermNumber()
                                                {
                                                    Value = 20
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
                                                            Value = "level"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
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
                                                Value = "partial"
                                            },
                                            new AstTermString()
                                            {
                                                Value = "example"
                                            },
                                            new AstTermString()
                                            {
                                                Value = "allow"
                                            }
                                        }
                                    },
                                    new AstTermVar()
                                    {
                                        Value = "d"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Assert.AreEqual(expected, result);
            
        }


        [Test]
        public void TestPreparePartial()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
                OpaStore opaStore = opaClientEmbedded.OpaStore;
                var txn = opaStore.NewTransaction(true);
                txn.UpsertPolicy("policy", moduleData);
                txn.Commit();
                
                opaClientEmbedded.PreparePartial("data.example.allow == true");
            });
        }

        [Test]
        public void TestRunPreparedPartial()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
                OpaStore opaStore = opaClientEmbedded.OpaStore;
                var txn = opaStore.NewTransaction(true);
                txn.UpsertPolicy("policy", moduleData);
                txn.Commit();

                var preparedPartial = opaClientEmbedded.PreparePartial("data.example.allow == true");

                await preparedPartial.Partial(new
                {
                    subject = new {
                    clearance_level = 20
                    }
                }, new List<string>()
                {
                    "data.reports"
                });
            });
        }

        private static string moduleData = @"
        package example

        allow {
          input.subject.clearance_level >= data.reports[_].clearance_level
          input.subject.clearance_level <= data.reports[_].test_level
        }

        # Allow if the user is a member
        allow {
          data.reports[_].members[_] = data.roles.identity
          data.reports[_].members[_] = input.subject.login
        }

        # Allow if the user has a business area role
        # User requires a level above 20
        allow {
          some i, k
          data.reports[k].test = data.reports[k].asd
          data.reports[k].businessAreaId = input.subject.roles[i].businessAreaId
          data.reports[k].teamId = input.subject.roles[i].businessAreaId
          input.subject.roles[i].level >= 20
        }
        ";
    }
}