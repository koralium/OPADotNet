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
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.AspNetCore.Input
{
    /// <summary>
    /// Class that contains the input that will be sent to OPA
    /// </summary>
    class OpaInput
    {
        /// <summary>
        /// The subject that is doing the operation
        /// </summary>
        [JsonPropertyName("subject")]
        public OpaInputUser Subject { get; set; }

        [JsonPropertyName("operation")]
        public string Operation { get; set; }

        [JsonPropertyName("request")]
        public OpaInputRequest Request { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Extensions { get; set; }
    }
}
