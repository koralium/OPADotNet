using OPADotNet.Reasons;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Reasons
{
    public interface IReasonFormatter
    {
        string FormatReason(ReasonMessage reason);
    }
}
