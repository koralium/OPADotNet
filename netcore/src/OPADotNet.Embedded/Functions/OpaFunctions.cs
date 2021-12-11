using Newtonsoft.Json.Linq;
using OPADotNet.Ast.Models;
using OPADotNet.Embedded.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Functions
{
    public static class OpaFunctions
    {
        private static ArgumentVisitor ArgumentVisitor = new ArgumentVisitor();
        public static void RegisterFunction<TArg1, TOut>(string name, Func<TArg1, TOut> function)
        {
            RegoWrapper.RegisterFunction1(name, (term) =>
            {
                var token = ArgumentVisitor.Visit(term);
                var arg1 = token.ToObject<TArg1>();
                var result = function(arg1);
                if (result == null)
                {
                    return null;
                }
                var resultToken = JToken.FromObject(result);
                return TokenToTermHelper.Convert(resultToken);
            });
        }
    }
}
