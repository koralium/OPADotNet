using NUnit.Framework;
using OPADotNet.AspNetCore.Reasons;
using OPADotNet.Reasons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.AspNetCore.Tests.Reasons
{
    internal class DefaultFormatterTests
    {
        [Test]
        public void TestFormatMessageSingleLine()
        {
            DefaultReasonFormatter defaultReasonFormatter = new DefaultReasonFormatter();
            string actual = defaultReasonFormatter.FormatReason(new ReasonMessage("test1"));
            Assert.AreEqual("test1", actual);
        }

        [Test]
        public void TestFormatMessageSingleChild()
        {
            DefaultReasonFormatter defaultReasonFormatter = new DefaultReasonFormatter();
            string actual = defaultReasonFormatter.FormatReason(new ReasonMessage("test1", new List<ReasonAndCondition>()
            {
                new ReasonAndCondition(new List<ReasonMessage>(){ new ReasonMessage("test2") })
            }));
            var expected = "test1\r\n* test2";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestFormatMessageMultipleAnd()
        {
            DefaultReasonFormatter defaultReasonFormatter = new DefaultReasonFormatter();
            string actual = defaultReasonFormatter.FormatReason(new ReasonMessage("test1", new List<ReasonAndCondition>()
            {
                new ReasonAndCondition(new List<ReasonMessage>(){ new ReasonMessage("test2") }),
                new ReasonAndCondition(new List<ReasonMessage>(){ new ReasonMessage("test3") })
            }));
            var expected = "test1\r\nAll must be true:\r\n* test2\r\n* test3";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestFormatMessageMultipleOr()
        {
            DefaultReasonFormatter defaultReasonFormatter = new DefaultReasonFormatter();
            string actual = defaultReasonFormatter.FormatReason(new ReasonMessage("test1", new List<ReasonAndCondition>()
            {
                new ReasonAndCondition(new List<ReasonMessage>(){ new ReasonMessage("test2"), new ReasonMessage("test3") }),
            }));
            var expected = "test1\r\n* One must be true:\r\n  - test2\r\n  - test3";
            Assert.AreEqual(expected, actual);
        }
    }
}
