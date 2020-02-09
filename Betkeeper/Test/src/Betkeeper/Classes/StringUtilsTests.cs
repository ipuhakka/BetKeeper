using System.Collections.Generic;
using Betkeeper.Classes;
using NUnit.Framework;

namespace Betkeeper.Test.Classes
{
    [TestFixture]
    public class StringUtilsTests
    {
        [Test]
        public void GenerateRandomString_ReturnsStringOfSpecifiedLength()
        {
            new List<int>
            {
                15,
                12,
                10
            }.ForEach(length =>
            {
                var randomString = StringUtils.GenerateRandomString(length);

                Assert.AreEqual(length, randomString.Length);
            });
        }
    }
}
