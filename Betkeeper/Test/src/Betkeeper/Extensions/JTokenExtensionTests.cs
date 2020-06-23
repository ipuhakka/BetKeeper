using System;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Betkeeper.Extensions;

namespace Test.src.Betkeeper.Extensions
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
    }
}
