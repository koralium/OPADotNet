using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded
{
    public class OpaCompiler : IDisposable
    {
        internal readonly int compilerId;
        private bool disposedValue;

        public OpaCompiler(IReadOnlyDictionary<string, string> modules)
        {
            compilerId = RegoWrapper.CompileModules(JsonSerializer.Serialize(modules));
        }

        /// <summary>
        /// Returns all compiled policies
        /// </summary>
        public Dictionary<string, AstPolicy> GetPolicies()
        {
            int result = RegoWrapper.GetModules(compilerId);

            if (result < 0)
            {
                var error = RegoWrapper.GetString(result);
                throw new InvalidOperationException(error);
            }
            var data = RegoWrapper.GetString(result);
            return PartialJsonConverter.ReadEmbeddedPolicies(data);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                RegoWrapper.RemoveCompiler(compilerId);
                disposedValue = true;
            }
        }

        ~OpaCompiler()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
