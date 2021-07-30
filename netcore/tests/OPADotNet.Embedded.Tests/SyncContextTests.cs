using NUnit.Framework;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Tests
{
    public class SyncContextTests
    {
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

        private static string rbacModule = @"
package rbac

# By default, deny requests.
default allow = false

# Allow admins to do anything.
allow {
	user_is_admin
}

# Allow the action if the user is granted permission to perform the action.
allow {
	# Find grants for the user.
	some grant
	user_is_granted[grant]

	# Check if the grant permits the action.
	input.action == grant.action
	input.type == grant.type
}

# user_is_admin is true if...
user_is_admin {

	# for some `i`...
	some i

	# ""admin"" is the `i`-th element in the user->role mappings for the identified user.
	data.user_roles[input.user][i] == ""admin""
}

# user_is_granted is a set of grants for the user identified in the request.
# The `grant` will be contained if the set `user_is_granted` for every...
    user_is_granted[grant] {
	some i, j

    # `role` assigned an element of the user_roles for this user...
    role := data.user_roles[input.user][i]

# `grant` assigned a single grant from the grants list for 'role'...
    grant := data.role_grants[role][j]
}
";

        [Test]
        public void TestGetDataSets()
        {
            OpaClientEmbedded opaClientEmbedded = new OpaClientEmbedded(new OpaStore());
            SyncContext syncContext = new SyncContext(new List<SyncPolicyDescriptor>()
            {
                new SyncPolicyDescriptor()
                {
                    PolicyName = "example",
                    Unknown = "reports"
                },
                new SyncPolicyDescriptor()
                {
                    PolicyName = "rbac",
                    Unknown = "not-used"
                }
            }, opaClientEmbedded);

            var policyStep = syncContext.NewIteration();

            var policy = policyStep.CompilePolicy("test", moduleData);
            policyStep.AddPolicy(policy);

            policyStep.AddPolicy(policyStep.CompilePolicy("rbac", rbacModule));

            var dataStep = policyStep.Next();


        }
    }
}
