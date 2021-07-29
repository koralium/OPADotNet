using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet
{
    /// <summary>
    /// Represents a prepared partial evaluation.
    /// </summary>
    public interface IPreparedPartial : IDisposable
    {
        public Task<AstQueries> Partial(object input, IEnumerable<string> unknowns);
    }
}
