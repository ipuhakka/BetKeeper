using Betkeeper.Classes;
using NUnit.Framework;
using System;

namespace Betkeeper.Test.Classes
{
    [TestFixture]
    public class EnumTests
    {

        [Test]
        public void FromString_ValueNotFound_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => EnumHelper.FromString<Test>("oksfdk"));
        }

        [Test]
        public void FromString_IgnoresCasing()
        {
            var result = EnumHelper.FromString<Test>("tEST2");

            Assert.AreEqual(Test.Test2, result);
        }

        enum Test
        {
            Test1,
            Test2,
            Test3
        }
    }
}
