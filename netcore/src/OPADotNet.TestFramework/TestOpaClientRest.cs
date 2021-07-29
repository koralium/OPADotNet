﻿using OPADotNet.Ast.Models;
using OPADotNet.Embedded;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.TestFramework
{
    public class TestOpaClientRest : RestOpaClient
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly List<Policy> _policies;
        public TestOpaClientRest(OpaClientEmbedded opaClientEmbedded, List<Policy> policies) : base("http://localhost")
        {
            _opaClientEmbedded = opaClientEmbedded;
            _policies = policies;
        }

        public override Task<List<Policy>> GetPolicies()
        {
            return Task.FromResult(_policies.ToList());
        }

        public override IPreparedPartial PreparePartial(string query)
        {
            return _opaClientEmbedded.PreparePartial(query);
        }
    }
}
