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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Tests
{
    public class OpaCompilerTests
    {
        [Test]
        public void TestAddPolicy()
        {
            string module = @"
                package test

                allow {
                    some i, j
                    data.t[i].list[j] = ""test1""
                    data.t[i].list[j] = ""test2""
                }";
            OpaCompiler opaCompiler = new OpaCompiler(new Dictionary<string, string>()
            {
                {"test", module}
            });

            Assert.That(opaCompiler.compilerId > 0);
        }

        [Test]
        public void TestAddNonFunctionalPolicy()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                //Test having an unused variable
                string module = @"
                package test

                allow {
                    some i, j
                    data.t[i].list[_] = ""test1""
                    data.t[i].list[_] = ""test2""
                }";
                new OpaCompiler(new Dictionary<string, string>()
                {
                    {"test", module}
                });
            });
        }
    }
}
