using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet
{
    public interface IPreparedEvaluation : IDisposable
    {
        Task<IEnumerable<TBinding>> Evaluate<TBinding>(object input = null);
    }
}
