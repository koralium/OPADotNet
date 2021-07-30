using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Requirements
{
    internal class OpaPolicyRequirement : IAuthorizationRequirement
    {
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

        public string GetQuery()
        {
            if (_options.CustomQuery != null)
            {
                return _options.CustomQuery;
            }
            string policyName = PrependData(ModuleName);
            return $"{policyName}.allow == true";
        }

        public string GetUnknown()
        {
            if(_options.CustomUnknown != null)
            {
                return _options.CustomUnknown;
            }
            return PrependData(DataName);
        }

        public string GetInputResourceName()
        {
            return _options.InputResourceName;
        }

        public OpaPolicyRequirement(string moduleName, string dataName, string operation, OpaPolicyRequirementOptions options)
        {
            ModuleName = moduleName;
            DataName = dataName;
            Operation = operation;
            _options = options;
        }
    }
}
