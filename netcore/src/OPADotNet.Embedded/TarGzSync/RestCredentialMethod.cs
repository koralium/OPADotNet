using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.TarGzSync
{
    public abstract class RestCredentialMethod
    {
        internal abstract Task Apply(HttpRequestMessage httpRequestMessage);
    }
}
