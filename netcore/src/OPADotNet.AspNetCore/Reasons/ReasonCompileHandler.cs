using OPADotNet.Embedded;
using OPADotNet.Embedded.sync;
using OPADotNet.Models;
using OPADotNet.Reasons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.AspNetCore.Reasons
{
    internal class ReasonCompileHandler : ISyncDoneHandler
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly IReasonHandler _reasonHandler;

        public ReasonCompileHandler(OpaClientEmbedded opaClientEmbedded, IReasonHandler reasonHandler)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _reasonHandler = reasonHandler;
        }

        public async Task SyncDone()
        {
            // TODO: Move requirements store into the service collection and make it possible to add handlers that the reason compiler can use
            // when a new requirement is added.
            var requirements = RequirementsStore.GetRequirements();

            await _reasonHandler.Load(requirements.Select(x => x.GetQuery()));
            var policies = await _opaClientEmbedded.GetPolicies();
        }
    }
}
