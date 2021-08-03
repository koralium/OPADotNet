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
using OPADotNet.Core.RestAPI.Models;
using OPADotNet.Partial.Ast;
using OPADotNet.RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OPADotNet.RestAPI
{
    public class RestOpaClient : IOpaClient
    {
        private readonly Uri _httpUrl;
        public RestOpaClient(string httpUrl)
        {
            _httpUrl = new Uri(httpUrl);
        }

        public RestOpaClient(Uri uri)
        {
            _httpUrl = uri;
        }

        public virtual async Task<List<Policy>> GetPolicies()
        {
            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync(new Uri(_httpUrl, "/v1/policies"));
            var content = await result.Content.ReadAsStringAsync();
            var parsed = PartialJsonConverter.ReadPolicyResponse(content);
            return parsed.Result;
        }

        public virtual async Task<T> GetData<T>(string path)
        {
            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync(new Uri(_httpUrl, "/v1/data" + path));
            var content = await result.Content.ReadAsStringAsync();
            var deserialized = JsonSerializer.Deserialize<DataResponse<T>>(content);
            return deserialized.Result;
        }

        public virtual async Task<string> GetDataJson(string path)
        {
            var jsonData = await GetData<JsonElement>(path);
            return jsonData.GetRawText();
        }

        public virtual IPreparedPartial PreparePartial(string query)
        {
            return new RestPreparedPartial(this, query);
        }

        internal virtual async Task<AstQueries> Compile(string query, object input, List<string> unknowns)
        {
            var httpClient = new HttpClient();
            var result = await httpClient.PostAsJsonAsync(new Uri(_httpUrl, "/v1/compile"), new CompileRequest()
            {
                Input = input,
                Query = query,
                Unknowns = unknowns
            });

            var content = await result.Content.ReadAsStringAsync();
            return PartialJsonConverter.ReadCompileResponse(content).Result;
        }
    }
}
