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
            var expected = $"test1{Environment.NewLine}* test2";
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
            var expected = $"test1{Environment.NewLine}All must be true:{Environment.NewLine}* test2{Environment.NewLine}* test3";
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
            var expected = $"test1{Environment.NewLine}* One must be true:{Environment.NewLine}  - test2{Environment.NewLine}  - test3";
            Assert.AreEqual(expected, actual);
        }
    }
}
