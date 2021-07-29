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
                OpaStore opaStore = new OpaStore();
            });
        }

        [Test]
        public void TestCreateTransaction()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaStore();
                opaStore.NewTransaction();
            });
        }

        [Test]
        public void TestUpsertPolicy()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaStore();
                var transaction = opaStore.NewTransaction();
                transaction.UpsertPolicy("policy", moduleData);
            });
        }

        [Test]
        public void TestUpsertPolicyAndCommit()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaStore();
                var transaction = opaStore.NewTransaction();
                transaction.UpsertPolicy("policy", moduleData);
                transaction.Commit();
            });
        }

        [Test]
        public void TestWrite()
        {
            Assert.DoesNotThrow(() =>
            {
                OpaStore opaStore = new OpaStore();
                var transaction = opaStore.NewTransaction();
                transaction.Write("/roles", new
                {
                    test = "test"
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
