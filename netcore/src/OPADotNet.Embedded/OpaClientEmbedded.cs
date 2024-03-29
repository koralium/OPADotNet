﻿/*
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
using OPADotNet;
using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using OPADotNet.Core.Models;
using OPADotNet.Embedded.Internal;
using OPADotNet.Embedded.utils;
using OPADotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded
{
    public partial class OpaClientEmbedded : IOpaClient
    {
        private readonly OpaStore _opaStore;
        private readonly List<WeakReference<IPreparedEmbedded>> _prepared = new List<WeakReference<IPreparedEmbedded>>();

        public OpaClientEmbedded()
        {
            _opaStore = new OpaStore(this);
        }

        public OpaStore OpaStore => _opaStore;

        internal void UpdatePrepared()
        {
            for (int i = 0; i < _prepared.Count; i++)
            {
                if (_prepared[i].TryGetTarget(out var target))
                {
                    target.Update();
                }
                else
                {
                    _prepared.RemoveAt(i);
                    i--;
                }
            }
        }

        public IPreparedPartial PreparePartial(string query)
        {
            var preparedPartial = new PreparedPartialEmbedded(OpaStore, query);
            _prepared.Add(new WeakReference<IPreparedEmbedded>(preparedPartial));
            return preparedPartial;
        }

        public IPreparedEvaluation PrepareEvaluation(string query)
        {
            var preparedEval = new PreparedEvalEmbedded(OpaStore, query);
            _prepared.Add(new WeakReference<IPreparedEmbedded>(preparedEval));
            return preparedEval;
        }

        /// <summary>
        /// Does a partial evaluation with all used data sources as unknown together with the input.
        /// This allows to get a picture of what data the policy uses and how its execution will be handled.
        /// </summary>
        /// <param name="query">The query to do the partial evaluation on.</param>
        /// <returns></returns>
        public Task<PartialResult> FullPartial(string query)
        {
            return FullPartialUtils.FullPartial(this, query);
        }

        public Task<IReadOnlyList<Policy>> GetPolicies()
        {
            var policies = _opaStore.GetCompiler().GetPolicies();
            List<Policy> output = new List<Policy>();
            foreach (var module in _opaStore._modules)
            {
                if (policies.TryGetValue(module.Key, out var astPolicy))
                {
                    output.Add(new Policy(module.Key, module.Value, astPolicy));
                }
            }
            return Task.FromResult<IReadOnlyList<Policy>>(output);
        }
    }
}
