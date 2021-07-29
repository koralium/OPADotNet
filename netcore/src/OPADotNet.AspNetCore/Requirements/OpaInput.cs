using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.AspNetCore.Requirements
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

        [JsonExtensionData]
        public Dictionary<string, object> Extensions { get; set; }
    }
}
