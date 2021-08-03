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
using System.Text;
using System.Text.Json;

namespace OPADotNet.Embedded.Sync
{
    public class LocalSyncOptions
    {
        internal List<string> Policies { get; } = new List<string>();

        internal List<LocalData> LocalDatas { get; } = new List<LocalData>();

        public LocalSyncOptions AddPolicy(string policyText)
        {
            Policies.Add(policyText);
            return this;
        }

        public LocalSyncOptions AddData(string path, string content)
        {
            LocalDatas.Add(new LocalData(path, content));
            return this;
        }

        public LocalSyncOptions AddData<T>(string path, T input)
        {
            string content = JsonSerializer.Serialize(input);
            return AddData(path, content);
        }
    }
}
