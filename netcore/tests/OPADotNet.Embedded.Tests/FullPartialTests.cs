using NUnit.Framework;
using OPADotNet.AspNetCore.Reasons;
using OPADotNet.Ast.Models;
using OPADotNet.Core.Ast.Explanation;
using OPADotNet.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    internal class FullPartialTests
    {
        [Test]
        public async Task TestSingleBlock()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            OpaStore opaStore = opaClientEmbedded.OpaStore;
            var txn = opaStore.NewTransaction(true);
            txn.UpsertPolicy("policy", @"
            package example

            allow {
                input.subject.name = data.name.testing
            }
            ");
            txn.Commit();

            var partialResult = await opaClientEmbedded.FullPartial("data.example.allow == true");

            var expected = new PartialResult()
            {
                Explanation = new List<ExplanationNode>()
                {
                    new ExplanationBody()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation(){ File = "query", Row = 1 },
                        Message = null,
                        Node = new AstBody()
                        {
                            Expressions = new List<AstExpression>()
                            {
                                new AstExpression()
                                {
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
                                                    Value = "example"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "allow"
                                                }
                                            }
                                        },
                                        new AstTermBoolean()
                                        {
                                            Value = true
                                        }
                                    },
                                    Index = 0
                                }
                            }
                        },
                        Operation = "enter",
                        ParentId = 0,
                        QueryId = 0
                    },
                    new ExplanationExpression()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation(){ File = "query", Row = 1 },
                        Message = null,
                        Node = new AstExpression()
                        {
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
                                            Value = "example"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "allow"
                                        }
                                    }
                                },
                                new AstTermBoolean()
                                {
                                    Value = true
                                }
                            },
                            Index = 0
                        },
                        Operation = "eval",
                        ParentId = 0,
                        QueryId = 0
                    },
                    new ExplanationExpression()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation(){ File = "query", Row = 1 },
                        Message = "(matched 1 rule)",
                        Node = new AstExpression()
                        {
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
                                            Value = "example"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "allow"
                                        }
                                    }
                                },
                                new AstTermBoolean()
                                {
                                    Value = true
                                }
                            },
                            Index = 0
                        },
                        Operation = "index",
                        ParentId = 0,
                        QueryId = 0
                    },
                    new ExplanationRule()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation()
                        {
                            File = "policy.rego",
                            Row = 4
                        },
                        Message = null,
                        Operation = "enter",
                        ParentId = 0,
                        QueryId = 1,
                        Node = new AstPolicyRule()
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
                                                        Value = "input"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "subject"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "name"
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
                                                        Value = "name"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "testing"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ExplanationExpression()
                    {
                        QueryId = 1,
                        ParentId = 0,
                        Location = new ExplanationLocation()
                        {
                            File = "policy.rego",
                            Row = 5
                        },
                        Locals = new List<ExplanationBinding>(),
                        Message = null,
                        Operation = "eval",
                        Node = new AstExpression()
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
                                            Value = "input"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "subject"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "name"
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
                                            Value = "name"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "testing"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ExplanationExpression()
                    {
                        QueryId = 1,
                        ParentId = 0,
                        Location = new ExplanationLocation()
                        {
                            File = "policy.rego",
                            Row = 5
                        },
                        Locals = new List<ExplanationBinding>(),
                        Message = null,
                        Operation = "save",
                        Node = new AstExpression()
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
                                            Value = "input"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "subject"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "name"
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
                                            Value = "name"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "testing"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ExplanationRule()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation()
                        {
                            File = "policy.rego",
                            Row = 4
                        },
                        Message = null,
                        Operation = "exit",
                        ParentId = 0,
                        QueryId = 1,
                        Node = new AstPolicyRule()
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
                                                        Value = "input"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "subject"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "name"
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
                                                        Value = "name"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "testing"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ExplanationBody()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation(){ File = "query", Row = 1 },
                        Message = null,
                        Node = new AstBody()
                        {
                            Expressions = new List<AstExpression>()
                            {
                                new AstExpression()
                                {
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
                                                    Value = "example"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "allow"
                                                }
                                            }
                                        },
                                        new AstTermBoolean()
                                        {
                                            Value = true
                                        }
                                    },
                                    Index = 0
                                }
                            }
                        },
                        Operation = "exit",
                        ParentId = 0,
                        QueryId = 0
                    },
                    new ExplanationBody()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation(){ File = "query", Row = 1 },
                        Message = null,
                        Node = new AstBody()
                        {
                            Expressions = new List<AstExpression>()
                            {
                                new AstExpression()
                                {
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
                                                    Value = "example"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "allow"
                                                }
                                            }
                                        },
                                        new AstTermBoolean()
                                        {
                                            Value = true
                                        }
                                    },
                                    Index = 0
                                }
                            }
                        },
                        Operation = "redo",
                        ParentId = 0,
                        QueryId = 0
                    },
                    new ExplanationExpression()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation(){ File = "query", Row = 1 },
                        Message = null,
                        Node = new AstExpression()
                        {
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
                                            Value = "example"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "allow"
                                        }
                                    }
                                },
                                new AstTermBoolean()
                                {
                                    Value = true
                                }
                            },
                            Index = 0
                        },
                        Operation = "redo",
                        ParentId = 0,
                        QueryId = 0
                    },
                    new ExplanationRule()
                    {
                        Locals = new List<ExplanationBinding>(),
                        Location = new ExplanationLocation()
                        {
                            File = "policy.rego",
                            Row = 4
                        },
                        Message = null,
                        Operation = "redo",
                        ParentId = 0,
                        QueryId = 1,
                        Node = new AstPolicyRule()
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
                                                        Value = "input"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "subject"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "name"
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
                                                        Value = "name"
                                                    },
                                                    new AstTermString()
                                                    {
                                                        Value = "testing"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new ExplanationExpression()
                    {
                        QueryId = 1,
                        ParentId = 0,
                        Location = new ExplanationLocation()
                        {
                            File = "policy.rego",
                            Row = 5
                        },
                        Locals = new List<ExplanationBinding>(),
                        Message = null,
                        Operation = "redo",
                        Node = new AstExpression()
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
                                            Value = "input"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "subject"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "name"
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
                                            Value = "name"
                                        },
                                        new AstTermString()
                                        {
                                            Value = "testing"
                                        }
                                    }
                                }
                            }
                        }
                    },
                },
                Result = new AstQueries()
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
                                                    Value = "input"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "subject"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "name"
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
                                                    Value = "name"
                                                },
                                                new AstTermString()
                                                {
                                                    Value = "testing"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            Assert.AreEqual(expected, partialResult);
        }
    }
}
