using NUnit.Framework;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded.Functions;
using OPADotNet.Embedded.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.Tests
{
    internal class FunctionTests
    {
        private class ReturnVal
        {
            public List<String> List { get; set; }
        }

        [Test]
        public void TestRegisterFunction1()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                OpaFunctions.RegisterFunction<string, object>("test", (arg) =>
                {
                    return new { a = 123 };
                });
                OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();

                RegoWrapper.RegisterRemoteStore(opaClientEmbedded.OpaStore._storeId, "test", (path) =>
                {
                    return "{ \"a\": {}}";
                });

                OpaStore opaStore = opaClientEmbedded.OpaStore;
                var txn = opaStore.NewTransaction(true);
                txn.UpsertPolicy("policy", moduleData);
                txn.Commit();

                var prepared = opaClientEmbedded.PrepareEvaluation("List = data.example.violation");

                var o = (await prepared.Evaluate<ReturnVal>()).ToList();

                var actual = o.First().List.First();
                Assert.AreEqual("This is a message", actual);
                //o.FirstOrDefault()
            });
        }

        [Test]
        public void TestRegisterFunction1ReturnNull()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                OpaFunctions.RegisterFunction<string, object>("test", (arg) =>
                {
                    return null;
                });
                OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded();
                OpaStore opaStore = opaClientEmbedded.OpaStore;
                var txn = opaStore.NewTransaction(true);
                txn.UpsertPolicy("policy", moduleData);
                txn.Commit();

                var prepared = opaClientEmbedded.PrepareEvaluation("List = data.example.violation");

                var o = (await prepared.Evaluate<ReturnVal>()).ToList();

                var actual = o.First().List.First();
                Assert.AreEqual("This is a message", actual);
                //o.FirstOrDefault()
            });
        }

        private static string moduleData = @"
        package example

        violation[msg] {
            td := data.test
            tda := data.test.employee
            m := test(""test"")
            td.a = ""asd""
            msg := ""This is a message""
        }
        ";
    }
}
