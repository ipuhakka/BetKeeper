using System.Collections.Generic;
using Betkeeper.Extensions;
using Betkeeper.Models;
using Betkeeper.Pages.CompetitionPage;
using NUnit.Framework;

namespace Betkeeper.Test.Extensions
{
    [TestFixture]
    public class JArrayExtensionTests
    {
        [Test]
        public void ToJObject_Succeeds()
        {
            var targets = new List<Target>
            {
                new Target
                {
                    TargetId = 1,
                    CompetitionId = 1,
                    Bet = "Something"
                },
                new Target
                {
                    TargetId = 2,
                    CompetitionId = 1,
                    Bet = "Something 2"
                }
            };

            // Test through competitionpage target conversion
            var testJArray = CompetitionPage.TargetsToJArray(targets);

            var resultObject = testJArray.ToJObject();

            Assert.AreEqual(2, resultObject.Count);
        }
    }
}
