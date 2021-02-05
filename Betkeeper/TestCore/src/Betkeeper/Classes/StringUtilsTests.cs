using Betkeeper.Classes;
using NUnit.Framework;
using System.Collections.Generic;

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
