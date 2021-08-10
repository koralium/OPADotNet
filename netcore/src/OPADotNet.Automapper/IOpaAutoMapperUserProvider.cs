using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace OPADotNet.Automapper
{
    public interface IOpaAutoMapperUserProvider
    {
        ClaimsPrincipal GetClaimsPrincipal();
    }
}
