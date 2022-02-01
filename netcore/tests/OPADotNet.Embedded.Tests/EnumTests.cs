using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    public class EnumTests
    {
        private enum Enum
        {
            Value1 = 0,
            Value2 = 1
        }

        private class EvaluateResult
        {
            [JsonPropertyName("result")]
            public bool Result { get; set; }
        }

        [Test]
        public async Task TestEvaluateWithEnumMatch()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            OpaStore opaStore = opaClientEmbedded.OpaStore;
            var txn = opaStore.NewTransaction(true);
            txn.UpsertPolicy("policy", @"
            package example

            default allow = false
            allow {
                input.value = ""Value1""
            }
            ");
            txn.Commit();

            var preparedEval = opaClientEmbedded.PrepareEvaluation("result = data.example.allow");
            var result = await preparedEval.Evaluate<EvaluateResult>(new
            {
                Value = Enum.Value1
            });
            var actual = result.FirstOrDefault().Result;
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task TestEvaluateWithEnumFail()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            OpaStore opaStore = opaClientEmbedded.OpaStore;
            var txn = opaStore.NewTransaction(true);
            txn.UpsertPolicy("policy", @"
            package example

            default allow = false
            allow {
                input.value = ""Value4""
            }
            ");
            txn.Commit();

            var preparedEval = opaClientEmbedded.PrepareEvaluation("result = data.example.allow");
            var result = await preparedEval.Evaluate<EvaluateResult>(new
            {
                Value = Enum.Value1
            });
            var actual = result.FirstOrDefault().Result;
            Assert.AreEqual(false, actual);
        }

        [Test]
        public async Task TestPartialWithEnumMatch()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            OpaStore opaStore = opaClientEmbedded.OpaStore;
            var txn = opaStore.NewTransaction(true);
            txn.UpsertPolicy("policy", @"
            package example

            default allow = false
            allow {
                input.value = ""Value1""
            }
            ");
            txn.Commit();

            var preparedPartial = opaClientEmbedded.PreparePartial("data.example.allow == true");
            var result = await preparedPartial.Partial(new
            {
                Value = Enum.Value1
            }, new List<string>());

            Assert.NotNull(result.Result);
            Assert.NotNull(result.Result.Queries);
            Assert.AreEqual(1, result.Result.Queries.Count);
            var query = result.Result.Queries.First();
            Assert.AreEqual(0, query.Expressions.Count);
        }

        [Test]
        public async Task TestPartialWithEnumFail()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
            OpaStore opaStore = opaClientEmbedded.OpaStore;
            var txn = opaStore.NewTransaction(true);
            txn.UpsertPolicy("policy", @"
            package example

            default allow = false
            allow {
                input.value = ""Value4""
            }
            ");
            txn.Commit();

            var preparedPartial = opaClientEmbedded.PreparePartial("data.example.allow == true");
            var result = await preparedPartial.Partial(new
            {
                Value = Enum.Value1
            }, new List<string>());

            Assert.NotNull(result.Result);
            Assert.IsNull(result.Result.Queries);
        }
    }
}
