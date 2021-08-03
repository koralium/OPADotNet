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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.Internal
{
    internal class DictionaryComparer<TKey, TValue> :
    IEqualityComparer<IReadOnlyDictionary<TKey, TValue>>
    {
        private IEqualityComparer<TValue> valueComparer;
        public DictionaryComparer(IEqualityComparer<TValue> valueComparer = null)
        {
            this.valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        }
        public bool Equals(IReadOnlyDictionary<TKey, TValue> x, IReadOnlyDictionary<TKey, TValue> y)
        {
            if (x.Count != y.Count)
                return false;
            if (x.Keys.Except(y.Keys).Any())
                return false;
            if (y.Keys.Except(x.Keys).Any())
                return false;
            foreach (var pair in x)
                if (!valueComparer.Equals(pair.Value, y[pair.Key]))
                    return false;
            return true;
        }

        public int GetHashCode(IReadOnlyDictionary<TKey, TValue> obj)
        {
            throw new NotImplementedException();
        }
    }

}
