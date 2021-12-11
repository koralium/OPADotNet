using Newtonsoft.Json.Linq;
using OPADotNet.Ast;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.Functions
{
    internal class ArgumentVisitor : AstVisitor<JToken>
    {
        public override JToken VisitTermBoolean(AstTermBoolean termBoolean)
        {
            return JToken.FromObject(termBoolean.Value);
        }

        public override JToken VisitTermNumber(AstTermNumber partialTermNumber)
        {
            return JToken.FromObject(partialTermNumber.Value);
        }

        public override JToken VisitTermString(AstTermString partialTermString)
        {
            return JToken.FromObject(partialTermString.Value);
        }

        public override JToken VisitTermArray(AstTermArray termArray)
        {
            JArray array = new JArray();
            foreach(var item in termArray.Value)
            {
                array.Add(item.Accept(this));
            }
            return array;
        }

        public override JToken VisitTermObject(AstTermObject astTermObject)
        {
            JObject obj = new JObject();
            foreach(var property in astTermObject.Value)
            {
                if (property.Values.Count != 2)
                {
                    throw new Exception("Could not parse object, property length not equal to 2");
                }
                if (property.Values.First() is AstTermString propertyName)
                {
                    var token = property.Values.Last().Accept(this);
                    obj.Add(propertyName.Value, token);
                }
                else
                {
                    throw new Exception("Expected string as property name but got " + property.Values.First().Type.ToString());
                }
            }
            return obj;
        }

        public override JToken VisitTermVar(AstTermVar partialTermVar)
        {
            return JToken.FromObject(partialTermVar);
        }

        public override JToken VisitTermRef(AstTermRef partialTermRef)
        {
            return JToken.FromObject(partialTermRef);
        }
    }
}
