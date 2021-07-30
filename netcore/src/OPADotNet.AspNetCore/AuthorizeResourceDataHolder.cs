using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore
{
    internal class AuthorizeResourceDataHolder
    {
        public object Resource { get; }

        public object Data { get; }

        public AuthorizeResourceDataHolder(object resource, object data)
        {
            Resource = resource;
            Data = data;
        }
    }
}
