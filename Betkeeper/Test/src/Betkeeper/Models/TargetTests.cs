using Betkeeper.Data;
using Betkeeper.Models;
using NUnit.Framework;
using System.Collections.Generic;
using TestTools;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class TargetTests
    {
        private TargetRepository _targetRepository;
        private BetkeeperDataContext _context;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _context = Tools.GetTestContext();
            _targetRepository = new TargetRepository(_context);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _targetRepository.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Target.RemoveRange(_context.Target);
        }

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

            Tools.CreateTestData(_context, targets: targets);

            var resultTargets = _targetRepository.GetTargets(competitionId: 1);

            Assert.AreEqual(2, resultTargets.Count);
            resultTargets.ForEach(target =>
            {
                Assert.AreEqual(1, target.CompetitionId);
            });
        }

        [Test]
        public void ClearTargets_DeletesOnlySpecificCompetitionsTargets()
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

            Tools.CreateTestData(_context, targets: targets);

            _targetRepository.ClearTargets(competitionId: 1);

            var existingTargets = _targetRepository.GetTargets();

            Assert.AreEqual(1, existingTargets.Count);
            Assert.AreEqual(2, existingTargets[0].CompetitionId);
        }
    }
}
