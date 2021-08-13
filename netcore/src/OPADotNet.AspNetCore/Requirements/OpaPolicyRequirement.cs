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
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Requirements
{
    internal class OpaPolicyRequirement : IAuthorizationRequirement
    {
        private readonly List<string> _unknowns;
        private readonly string _unknown;
        private readonly OpaPolicyRequirementOptions _options;

        public string ModuleName { get; }

        public string DataName { get; }

        public string Operation { get; }

        /// <summary>
        /// Internal index to keep track of the requirements
        /// </summary>
        internal int Index { get; set; }

        private string PrependData(string name)
        {
            if (!name.StartsWith("data."))
            {
                return $"data.{name}";
            }
            return name;
        }

        internal string GetQuery()
        {
            if (_options.CustomQuery != null)
            {
                return _options.CustomQuery;
            }
            string policyName = PrependData(ModuleName);
            return $"{policyName}.allow == true";
        }

        internal List<string> GetUnknowns()
        {
            return _unknowns;
        }

        internal string GetUnknown()
        {
            return _unknown;
        }

        private string GetUnknownValue()
        {
            if(_options.CustomUnknown != null)
            {
                return _options.CustomUnknown;
            }
            if (DataName == null)
            {
                return null;
            }
            return PrependData(DataName);
        }

        internal string GetInputResourceName()
        {
            return _options.InputResourceName;
        }

        public OpaPolicyRequirement(string moduleName, string dataName, string operation, OpaPolicyRequirementOptions options)
        {
            ModuleName = moduleName;
            DataName = dataName;
            Operation = operation;
            _options = options;

            _unknown = GetUnknownValue();

            if (_unknown == null)
            {
                _unknowns = new List<string>();
            }
            else
            {
                _unknowns = new List<string>() { _unknown };
            }
        }
    }
}
