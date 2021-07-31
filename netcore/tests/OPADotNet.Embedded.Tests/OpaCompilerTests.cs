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
