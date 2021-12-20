using NUnit.Framework;
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
            txn.UpsertPolicy("pol1", @"
            package test

            allow = true
            ");
            txn.Commit();

            var policies = await opaClientEmbedded.GetPolicies();

        }
    }
}
