using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Tests
{
    class StoreTests
    {
        [Test]
        public void TestCreateStore()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaStore(null);
            });
        }

        [Test]
        public void TestCreateTransaction()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaStore(null);
                opaStore.NewTransaction(true);
            });
        }

        [Test]
        public void TestUpsertPolicy()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaClientEmbedded().OpaStore;
                var transaction = opaStore.NewTransaction(true);
                transaction.UpsertPolicy("policy", moduleData);
            });
        }

        [Test]
        public void TestUpsertPolicyAndCommit()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaClientEmbedded().OpaStore;
                var transaction = opaStore.NewTransaction(true);
                transaction.UpsertPolicy("policy", moduleData);
                transaction.Commit();
            });
        }

        [Test]
        public void TestWrite()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaClientEmbedded().OpaStore;
                var transaction = opaStore.NewTransaction(true);
                transaction.Write("/roles", new
                {
                    test = "test"
                });
            });
        }

        [Test]
        public void TestRead()
        {
            OpaStore opaStore = new OpaClientEmbedded().OpaStore;
            var transaction = opaStore.NewTransaction(true);
            transaction.Write("/roles", new
            {
                test = "test"
            });
            transaction.Commit();
            transaction = opaStore.NewTransaction(false);
            var content = transaction.Read("/roles");
            transaction.Commit();
            Assert.AreEqual("{\"test\":\"test\"}", content);
        }

        [Test]
        public void TestReadDuringWrite()
        {
            OpaStore opaStore = new OpaClientEmbedded().OpaStore;
            var transaction = opaStore.NewTransaction(true);
            transaction.Write("/roles", new
            {
                test = "test"
            });

            Assert.Throws<InvalidOperationException>(() =>
            {
                transaction = opaStore.NewTransaction(false);
                var content = transaction.Read("/roles");
                transaction.Commit();
            });
           
            transaction.Commit();
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
