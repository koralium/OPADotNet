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
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Ast.Models
{
    public abstract class AstTerm : AstNode
    {
        public abstract AstTermType Type { get; }

        public static AstTermString String(string value)
        {
            return new AstTermString()
            {
                Value = value
            };
        }

        public static AstTermNumber Number(decimal number)
        {
            return new AstTermNumber()
            {
                Value = number
            };
        }

        public static AstTermArray Array(List<AstTerm> array)
        {
            return new AstTermArray()
            {
                Value = array
            };
        }

        public static AstTermBoolean Bool(bool value)
        {
            return new AstTermBoolean()
            {
                Value = value
            };
        }

        public static AstTermObject Object(List<AstObjectProperty> properties)
        {
            return new AstTermObject()
            {
                Value = properties
            };
        }
    }
}
