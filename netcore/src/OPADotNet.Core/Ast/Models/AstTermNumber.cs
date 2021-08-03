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

namespace OPADotNet.Ast.Models
{
    public class AstTermNumber : AstTerm
    {
        public decimal Value { get; set; }

        public override AstTermType Type => AstTermType.Number;

        public override T Accept<T>(AstVisitor<T> visitor)
        {
            return visitor.VisitTermNumber(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is AstTermNumber other)
            {
                return Equals(Value, other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
