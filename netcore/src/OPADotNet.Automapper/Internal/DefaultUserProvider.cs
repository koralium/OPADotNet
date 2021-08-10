using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace OPADotNet.Automapper.Internal
{
    class DefaultUserProvider : IOpaAutoMapperUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DefaultUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal GetClaimsPrincipal()
        {
            return _httpContextAccessor.HttpContext.User;
        }
    }
}
