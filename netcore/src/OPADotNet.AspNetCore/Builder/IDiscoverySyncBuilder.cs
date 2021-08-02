using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    public interface IDiscoverySyncBuilder : ISyncBuilder
    {
        /// <summary>
        /// The decision path to locate discovery data, default "data".
        /// </summary>
        string Decision { get; set; }
    }
}
