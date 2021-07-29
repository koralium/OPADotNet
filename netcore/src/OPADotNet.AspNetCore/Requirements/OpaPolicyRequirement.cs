using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.AspNetCore.Requirements
{
    internal class OpaPolicyRequirement : IAuthorizationRequirement
    {
        public string ModuleName { get; }

        public string DataName { get; }

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
            string policyName = PrependData(ModuleName);
            return $"{policyName}.allow == true";
        }

        public string GetUnknown()
        {
            return PrependData(DataName);
        }

        public OpaPolicyRequirement(string moduleName, string dataName)
        {
            ModuleName = moduleName;
            DataName = dataName;
        }
    }
}
