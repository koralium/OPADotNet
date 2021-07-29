using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet
{
    public interface IOpaClient
    {
        IPreparedPartial PreparePartial(string query);
    }
}
