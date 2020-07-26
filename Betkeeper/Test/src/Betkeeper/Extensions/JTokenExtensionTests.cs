using System;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Betkeeper.Extensions;

namespace Betkeeper.Test.Extensions
{
    [TestFixture]
    public class JTokenExtensionTests
    {
        [Test]
        public void IsNullOrWhiteSpace_WhiteSpaceReturnsTrue()
        {
            Assert.IsTrue(new JValue("  ").IsNullOrWhiteSpace());
        }

        [Test]
        public void IsNullOrWhiteSpace_EmptyReturnsTrue()
        {
            Assert.IsTrue(new JValue("").IsNullOrWhiteSpace());
        }

        [Test]
        public void IsNullOrWhiteSpace_NullReturnsTrue()
        {
            Assert.IsTrue(new JValue((string)null).IsNullOrWhiteSpace());
        }

        [Test]
        public void IsNullOrWhiteSpace_ContainsValueReturnsTrue()
        {
            Assert.IsFalse(new JValue(true).IsNullOrWhiteSpace());
            Assert.IsFalse(new JValue(1).IsNullOrWhiteSpace());
            Assert.IsFalse(new JValue("Not empty").IsNullOrWhiteSpace());
        }

        [Test]
        public void ToDoubleInvariantCulture_InputContainsCommas_ParsesCorrectly()
        {
            Assert.AreEqual(2.78, new JValue("2,78").GetDoubleInvariantCulture());
        }

        [Test]
        public void ToDoubleInvariantCulture_InputContainsPoint_ParsesCorrectly()
        {
            Assert.AreEqual(2.78, new JValue("2.78").GetDoubleInvariantCulture());
        }

        [Test]
        public void ToDoubleInvariantCulture_InputNull_ReturnsNull()
        {
            Assert.IsNull(new JValue((object)null).GetDoubleInvariantCulture());
        }

        [Test]
        public void ToDoubleInvariantCulture_InputNotNumber_ReturnsNull()
        {
            Assert.IsNull(new JValue("test2.2").GetDoubleInvariantCulture());
            Assert.IsNull(new JValue(true).GetDoubleInvariantCulture());
            Assert.IsNull(new JValue("2.2asfd").GetDoubleInvariantCulture());
        }
    }
}
