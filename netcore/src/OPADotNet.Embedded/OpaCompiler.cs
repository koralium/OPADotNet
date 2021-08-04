/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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

            if (compilerId < 0)
            {
                var error = RegoWrapper.GetString(compilerId);
                throw new ArgumentException($"Error in provided modules: {error}", nameof(modules));
            }
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
