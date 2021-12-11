using Newtonsoft.Json.Linq;
using OPADotNet.Ast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.Functions
{
    internal static class TokenToTermHelper
    {
        
        public static AstTerm Convert(JToken token)
        {
            if (token.Type == JTokenType.Null)
            {
                return null;
            }
            if (token.Type == JTokenType.String)
            {
                return new AstTermString()
                {
                    Value = token.Value<string>()
                };
            }
            if (token.Type == JTokenType.Boolean)
            {
                return new AstTermBoolean()
                {
                    Value = token.Value<bool>()
                };
            }
            if (token.Type == JTokenType.Integer)
            {
                return new AstTermNumber()
                {
                    Value = token.Value<long>()
                };
            }
            if (token.Type == JTokenType.Date)
            {
                return new AstTermString()
                {
                    Value = token.ToString()
                };
            }
            if (token.Type == JTokenType.Float)
            {
                return new AstTermNumber()
                {
                    Value = (decimal)token.Value<double>()
                };
            }
            if (token.Type == JTokenType.Guid)
            {
                return new AstTermString()
                {
                    Value = token.ToString()
                };
            }
            if (token.Type == JTokenType.Bytes)
            {
                return new AstTermString()
                {
                    Value = token.ToString()
                };
            }
            if (token.Type == JTokenType.TimeSpan)
            {
                return new AstTermString()
                {
                    Value = token.ToString()
                };
            }
            if (token.Type == JTokenType.Uri)
            {
                return new AstTermString()
                {
                    Value = token.ToString()
                };
            }

            if (token.Type == JTokenType.Array)
            {
                AstTermArray array = new AstTermArray()
                {
                    Value = new List<AstTerm>()
                };
                var arr = (JArray)token;
                foreach(var t in arr)
                {
                    array.Value.Add(Convert(t));
                }
                return array;
            }

            if (token.Type == JTokenType.Object)
            {
                AstTermObject obj = new AstTermObject()
                {
                    Value = new List<AstObjectProperty>()
                };
                var o = (JObject)token;

                foreach(var p in o)
                {
                    var convertedPropertyValue = Convert(p.Value);

                    if (convertedPropertyValue == null)
                    {
                        continue;
                    }

                    var property = new AstObjectProperty()
                    {
                        Values = new List<AstTerm>()
                    };

                    property.Values.Add(new AstTermString()
                    {
                        Value = p.Key
                    });
                    property.Values.Add(convertedPropertyValue);
                    obj.Value.Add(property);
                }
                return obj;
            }
            throw new NotImplementedException();
        }
    }
}
