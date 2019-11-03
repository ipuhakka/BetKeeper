using System;
using Betkeeper.Extensions;
using NUnit.Framework;

namespace Betkeeper.Test.Extensions
{
    [TestFixture]
    public class DateTimeExtensionTests
    {

        [Test]
        public void Trim_RoundsToGivenAccuracy()
        {
            var test = new DateTime(2019, 1, 1, 14, 15, 22);

            test.AddMilliseconds(224);

            Assert.AreEqual(
                new DateTime(2019, 1, 1, 14, 15, 22),
                test.Trim(TimeSpan.TicksPerSecond));

            Assert.AreEqual(
                new DateTime(2019, 1, 1, 14, 15, 00),
                test.Trim(TimeSpan.TicksPerMinute));
        }
    }
}
