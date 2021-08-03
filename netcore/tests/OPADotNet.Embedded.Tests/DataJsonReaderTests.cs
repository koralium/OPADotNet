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
using OPADotNet.Embedded.sync;
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace OPADotNet.Embedded.Tests
{
    class DataJsonReaderTests
    {
        [Test]
        public void TestCopyRootObject()
        {
            string json = @"
                {
                    ""test"": ""testing""
                }
            ";
            var output = DataJsonReader.FilterJson(json, new DataSetNode("data", false));

            Assert.AreEqual(@"{""test"":""testing""}", output);
        }

        [Test]
        public void TestCopySinglePropertyFromObject()
        {
            string json = @"
                {
                    ""test"": ""testing"",
                    ""test2"": ""testing2""
                }
            ";

            var tree = new DataSetNode("data", false);
            tree.ChildrenMutable.Add("test", new DataSetNode("test", false));

            var output = DataJsonReader.FilterJson(json, tree);

            Assert.AreEqual(@"{""test"":""testing""}", output);
        }

        [Test]
        public void TestCopyFromObjectWithVariable()
        {
            string json = @"
                {
                    ""test"": ""testing"",
                    ""test2"": ""testing2""
                }
            ";

            var tree = new DataSetNode("data", false);
            tree.ChildrenMutable.Add("$0", new DataSetNode("$0", true));

            var output = DataJsonReader.FilterJson(json, tree);

            Assert.AreEqual(@"{""test"":""testing"",""test2"":""testing2""}", output);
        }

        [Test]
        public void TestCopyFromArrayRoot()
        {
            string json = @"[1, 2, 3]";
            var tree = new DataSetNode("data", false);

            var output = DataJsonReader.FilterJson(json, tree);
            Assert.AreEqual(@"[1,2,3]", output);
        }

        [Test]
        public void TestCopyFromArrayWithVariable()
        {
            string json = @"[1, 2, 3]";

            var tree = new DataSetNode("data", false);
            tree.ChildrenMutable.Add("$0", new DataSetNode("$0", true));

            var output = DataJsonReader.FilterJson(json, tree);

            Assert.AreEqual(@"[1,2,3]", output);
        }

        [Test]
        public void TestCopyFromEmptyArrayWithVariable()
        {
            string json = @"[]";

            var tree = new DataSetNode("data", false);
            tree.ChildrenMutable.Add("$0", new DataSetNode("$0", true));

            var output = DataJsonReader.FilterJson(json, tree);

            Assert.AreEqual(@"[]", output);
        }

        [Test]
        public void TestCopyFromEmptyArrayInsideObject()
        {
            string json = @"{""test"":[],""other"":""test""}";

            var tree = new DataSetNode("data", false);
            var testPropNode = new DataSetNode("test", false);
            tree.ChildrenMutable.Add("test", testPropNode);

            var output = DataJsonReader.FilterJson(json, tree);

            Assert.AreEqual(@"{""test"":[]}", output);
        }

        [Test]
        public void TestCopyFromArrayInsideObjectWithVariable()
        {
            string json = @"{""test"":[1, 2, 3],""other"":""test""}";

            var tree = new DataSetNode("data", false);
            var testPropNode = new DataSetNode("test", false);
            tree.ChildrenMutable.Add("test", testPropNode);
            testPropNode.ChildrenMutable.Add("$0", new DataSetNode("$0", true));

            var output = DataJsonReader.FilterJson(json, tree);

            Assert.AreEqual(@"{""test"":[1,2,3]}", output);
        }

        [Test]
        public void TestCopyFromArrayInsideObjectWithAllVariables()
        {
            string json = @"{""test"":[1, 2, 3],""other"":""test""}";

            var tree = new DataSetNode("data", false);
            var testPropNode = new DataSetNode("$0", true);
            tree.ChildrenMutable.Add("$0", testPropNode);
            testPropNode.ChildrenMutable.Add("$0", new DataSetNode("$0", true));

            var output = DataJsonReader.FilterJson(json, tree);

            Assert.AreEqual(@"{""test"":[1,2,3],""other"":""test""}", output);
        }

        [Test]
        public void TestCopyRootUsedInPolicy()
        {
            string json = @"{""test"":[1, 2, 3],""other"":""test""}";

            var tree = new DataSetNode("data", false)
            {
                UsedInPolicyMutable = true
            };
            var testPropNode = new DataSetNode("other", false)
            {
                UsedInPolicyMutable = true
            };
            tree.ChildrenMutable.Add("other", testPropNode);

            var output = DataJsonReader.FilterJson(json, tree);

            Assert.AreEqual(@"{""test"":[1,2,3],""other"":""test""}", output);
        }
    }
}
