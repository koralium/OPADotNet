using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Automapper.Tests
{
    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(params Claim[] claims) : base(new TestIdentity(claims))
        {
        }
    }

    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims) : base(claims)
        {
        }
    }


    class TestUserProvider : IOpaAutoMapperUserProvider
    {
        public ClaimsPrincipal GetClaimsPrincipal()
        {
            return new TestPrincipal(new Claim("name", "test"));
        }
    }
}
