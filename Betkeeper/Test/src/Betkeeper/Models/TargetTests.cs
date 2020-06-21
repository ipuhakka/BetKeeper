using Betkeeper.Models;
using NUnit.Framework;
using System.Collections.Generic;
using TestTools;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class TargetTests
    {
        [Test]
        public void GetTargets_FilterByCompetition_ReturnsCompetitionsTargets()
        {
            var targets = new List<Target>
            {
                new Target
                {
                    TargetId = 1,
                    CompetitionId = 1
                },
                new Target
                {
                    TargetId = 2,
                    CompetitionId = 1
                },
                new Target
                {
                    TargetId = 3,
                    CompetitionId = 2
                }
            };

            Tools.CreateTestData(targets: targets);

            var resultTargets = new TestTargetRepository().GetTargets(competitionId: 1);

            Assert.AreEqual(2, resultTargets.Count);
            resultTargets.ForEach(target =>
            {
                Assert.AreEqual(1, target.CompetitionId);
            });
        }
    }
}
