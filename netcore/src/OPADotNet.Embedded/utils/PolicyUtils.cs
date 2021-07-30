using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.utils
{
    internal static class PolicyUtils
    {
        public static Policy GetPolicy(string fileName, string rawText)
        {
            int result = RegoWrapper.CompilePolicy(fileName, rawText);

            if (result < 0)
            {
                var error = RegoWrapper.GetString(result);
                throw new InvalidOperationException(error);
            }

            var content = RegoWrapper.GetString(result);
            var astPolicy = PartialJsonConverter.ReadEmbeddedPolicy(content);
            return new Policy()
            {
                Id = fileName,
                Raw = rawText,
                Ast = astPolicy
            };
        }
    }
}
