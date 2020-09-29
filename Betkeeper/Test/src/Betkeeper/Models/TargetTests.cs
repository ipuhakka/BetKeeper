using System.Linq;
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
            _targetRepository = new TargetRepository();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Target.RemoveRange(_context.Target);
            _context.SaveChanges();
        }

        [Test]
        public void Target_TargetResultSet_ResultNull_ReturnsFalse()
        {
            var target = new Target
            {
                Result = null
            };

            Assert.IsFalse(target.TargetResultSet());
        }

        [Test]
        public void Target_TargetResultSet_ResultStringNotEmpty_ReturnsTrue()
        {
            var target = new Target
            {
                Result = new TargetResultItem
                {
                    Result = "test"
                }
            };

            Assert.IsTrue(target.TargetResultSet());
        }

        [Test]
        public void Target_TargetResultSet_DictionaryNotEmpty_ReturnsTrue()
        {
            var target = new Target
            {
                Result = new TargetResultItem
                {
                    TargetBetResultDictionary = new Dictionary<int, string>
                    {
                        {1, "test"}
                    }
                },
                Type = Enums.TargetType.OpenQuestion
            };

            Assert.IsTrue(target.TargetResultSet());
        }

        [Test]
        public void Target_TargetResultSet_DictionaryEmptyResultEmpty_ReturnsFalse()
        {
            var target = new Target
            {
                Result = new TargetResultItem
                {
                    TargetBetResultDictionary = new Dictionary<int, string>(),
                    Result = ""
                }
            };

            Assert.IsFalse(target.TargetResultSet());
        }

        [Test]
        public void Target_TargetResultSet_OpenQuestionSomeUnresolved_ReturnsFalse()
        {
            var target = new Target
            {
                Result = new TargetResultItem
                {
                    TargetBetResultDictionary = new Dictionary<int, string>
                    {
                        {1, "Wrong"},
                        {2, "Unresolved"}
                    },
                    Result = ""
                },
                Type = Enums.TargetType.OpenQuestion
            };

            Assert.IsFalse(target.TargetResultSet());
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

            Tools.CreateTestData(targets: targets);

            var resultTargets = _targetRepository.GetTargets(competitionId: 1);

            Assert.AreEqual(2, resultTargets.Count);
            resultTargets.ForEach(target =>
            {
                Assert.AreEqual(1, target.CompetitionId);
            });
        }

        [Test]
        public void GetTargets_FilterByIdList_ReturnsCorrectTargets()
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

            var resultTargets = _targetRepository.GetTargets(targetIds: new List<int>{ 1, 3 });

            Assert.AreEqual(2, resultTargets.Count);
            new List<int> { 1, 3 }.ForEach(value =>
            {
                Assert.IsTrue(resultTargets.Any(target => target.TargetId == value));
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

            Tools.CreateTestData(targets: targets);

            _targetRepository.ClearTargets(competitionId: 1);

            var existingTargets = _targetRepository.GetTargets();

            Assert.AreEqual(1, existingTargets.Count);
            Assert.AreEqual(2, existingTargets[0].CompetitionId);
        }

        [Test]
        public void RemoveTarget_TargetNotFound_NothingRemoves()
        {
            var targets = new List<Target>
            {
                new Target
                {
                    TargetId = 1,
                    CompetitionId = 1
                }
            };

            Tools.CreateTestData(targets: targets);

            _targetRepository.RemoveTarget(2);

            Assert.AreEqual(1, _targetRepository.GetTargets(1).Count);
        }

        [Test]
        public void RemoveTarget_TargetFound_TargetRemoved()
        {
            var targets = new List<Target>
            {
                new Target
                {
                    TargetId = 1,
                    CompetitionId = 1
                }
            };

            Tools.CreateTestData(targets: targets);

            _targetRepository.RemoveTarget(1);

            Assert.AreEqual(0, _targetRepository.GetTargets(1).Count);
        }

        [Test]
        public void GetPointInformation_TypeOpenQuestion_ReturnsCorrectInformation()
        {
            var target = new Target
            {
                TargetId = 1,
                Scoring = new List<Scoring>
                {
                    new Scoring{ Points = 2, Score = Enums.TargetScore.CorrectResult }
                },
                Type = Enums.TargetType.OpenQuestion
            };

            Assert.AreEqual("Correct: 2 points", target.GetPointInformation());
        }

        [Test]
        public void GetPointInformation_TypeResult_ReturnsCorrectInformation()
        {
            var target = new Target
            {
                TargetId = 1,
                Scoring = new List<Scoring>
                {
                    new Scoring{ Points = 2, Score = Enums.TargetScore.CorrectResult },
                    new Scoring{ Points = 1, Score = Enums.TargetScore.CorrectWinner }
                },
                Type = Enums.TargetType.Result
            };

            Assert.AreEqual("Result: 2 points, Winner: 1 points", target.GetPointInformation());
        }

        [Test]
        public void GetPointInformation_TypeSelection_ReturnsCorrectInformation()
        {
            var target = new Target
            {
                TargetId = 1,
                Scoring = new List<Scoring>
                {
                    new Scoring{ Points = 2, Score = Enums.TargetScore.CorrectResult }
                },
                Type = Enums.TargetType.Selection
            };

            Assert.AreEqual("Correct: 2 points", target.GetPointInformation());
        }
    }
}
