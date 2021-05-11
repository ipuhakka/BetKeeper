using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Models;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class TargetBetActionTests
    {
        private TargetBetAction _targetBetAction;
        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
            _targetBetAction = new TargetBetAction();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Target.RemoveRange(_context.Target);
            _context.TargetBet.RemoveRange(_context.TargetBet);
            _context.Competition.RemoveRange(_context.Competition);
            _context.Participator.RemoveRange(_context.Participator);

            _context.SaveChanges();
        }

        [Test]
        public void AddTargets_CompetitionNotOpen_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.UtcNow.AddDays(-1)
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var targetBets = new List<TargetBet>
            {
                new TargetBet{ Target = 1 }
            };

            try
            {
                _targetBetAction.SaveTargetBets(1, 1, targetBets);
                Assert.Fail("Failed");
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
                Assert.AreEqual("Competition is not open for betting", e.ErrorMessage);
            }
        }

        [Test]
        public void AddTargets_UserNotInCompetition_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.UtcNow.AddDays(1)
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var targetBets = new List<TargetBet>
            {
                new TargetBet{ Target = 1 }
            };

            try
            {
                _targetBetAction.SaveTargetBets(1, 1, targetBets);
                Assert.Fail("Failed");
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Unauthorized, e.ActionExceptionType);
                Assert.AreEqual("User not in competition", e.ErrorMessage);
            }
        }

        [Test]
        public void AddTargets_TargetDoesNotExist_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.UtcNow.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    UserId = 1,
                    ParticipatorId = 1,
                    Competition = 1
                }
            };

            Tools.CreateTestData(
                competitions: competitions, 
                participators: participators);

            var targetBets = new List<TargetBet>
            {
                new TargetBet{ Target = 1 }
            };

            try
            {
                _targetBetAction.SaveTargetBets(1, 1, targetBets);
                Assert.Fail("Failed");
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
                Assert.AreEqual("Target does not exist", e.ErrorMessage);
            }
        }

        [Test]
        public void SaveBetTargets_InvalidResult_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.UtcNow.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    UserId = 1,
                    ParticipatorId = 1,
                    Competition = 1
                }
            };

            var targets = new List<Target>
            {
                new Target
                {
                    TargetId = 1,
                    CompetitionId = 1,
                    Type = Enums.TargetType.Result
                }
            };

            Tools.CreateTestData(
                competitions: competitions,
                participators: participators,
                targets: targets);

            // Different types of invalid inputs
            var targetBets = new List<TargetBet>
            {
                new TargetBet{ Target = 1, Bet = "1" },
                new TargetBet{ Target = 1, Bet = "1-1-1" },
                new TargetBet{ Target = 1, Bet = "1--" },
                new TargetBet{ Target = 1, Bet = "-1" },
                new TargetBet{ Target = 1, Bet = "h-h" }
            };

            targetBets.ForEach(target =>
            {
                try
                {
                    _targetBetAction.SaveTargetBets(1, 1, new List<TargetBet> { target });
                    Assert.Fail("Failed");
                }
                catch (ActionException e)
                {
                    Assert.AreEqual(ActionResultType.InvalidInput, e.ActionExceptionType);
                    Assert.AreEqual("Target 1 has invalid result", e.ErrorMessage);
                }
            });
        }

        [Test]
        public void SaveBetTargets_SelectionNotAvailable_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.UtcNow.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    UserId = 1,
                    ParticipatorId = 1,
                    Competition = 1
                }
            };

            var targets = new List<Target>
            {
                new Target
                {
                    TargetId = 1,
                    CompetitionId = 1,
                    Type = Enums.TargetType.Selection,
                    Selections = new List<string>
                    {
                        "Selection1",
                        "Selection2"
                    }
                }
            };

            Tools.CreateTestData(
                competitions: competitions,
                participators: participators,
                targets: targets);

            var targetBets = new List<TargetBet>
            {
                new TargetBet{ Target = 1, Bet = "SelectionNotExisting" }
            };

            try
            {
                _targetBetAction.SaveTargetBets(1, 1, targetBets);
                Assert.Fail("Failed");
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.InvalidInput, e.ActionExceptionType);
                Assert.AreEqual("Target 1 has invalid selection", e.ErrorMessage);
            }
        }

        /// <summary>
        /// Tests that with valid inputs already saved target bets are updated and
        /// others created.
        /// </summary>
        [Test]
        public void SaveBetTargets_ValidInput_Succeeds()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.UtcNow.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    UserId = 1,
                    ParticipatorId = 1,
                    Competition = 1
                }
            };

            var targets = new List<Target>
            {
                new Target
                {
                    TargetId = 1,
                    CompetitionId = 1,
                    Type = Enums.TargetType.Result
                },
                new Target
                {
                    TargetId = 2,
                    CompetitionId = 1,
                    Type = Enums.TargetType.OpenQuestion
                },
                new Target
                {
                    TargetId = 3,
                    CompetitionId = 1,
                    Type = Enums.TargetType.Selection,
                    Selections = new List<string>()
                    {
                        "Barcelona",
                        "ManU"
                    }
                }
            };

            var targetBets = new List<TargetBet>
            {
                new TargetBet
                {
                    Target = 1,
                    Bet = "1-1",
                    Participator = 1
                },
                new TargetBet
                {
                    Target = 3,
                    Bet = "Barcelona",
                    Participator = 1
                }
            };

            Tools.CreateTestData(
                competitions: competitions,
                participators: participators,
                targets: targets,
                targetBets: targetBets);

            var newTargetBets = new List<TargetBet>
            {
                new TargetBet { Target = 1, Bet = "2-1" },
                new TargetBet { Target = 2, Bet = "Open answer"},
                new TargetBet { Target = 3, Bet = "ManU"}
            };

            // Verify two target bets in database before saving

            Assert.AreEqual(2, _context.TargetBet.Count());
            _targetBetAction.SaveTargetBets(1, 1, newTargetBets);

            var resultTargetBets = _context.TargetBet.ToList();

            // Check that correct number of target bets were created
            Assert.AreEqual(3, resultTargetBets.Count);

            new List<string> { "2-1", "ManU", "Open answer" }.ForEach(bet =>
            {
                Assert.IsTrue(resultTargetBets.Count(target => target.Bet == bet) == 1);
            });
        }

        [Test]
        public void GetCompetitionsTargetBets_ReturnsValidTargetBets()
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
                    CompetitionId = 2
                },
                new Target
                {
                    TargetId = 3,
                    CompetitionId = 1
                }
            };

            var targetBets = new List<TargetBet>
            {
                new TargetBet
                {
                    Target = 1
                },
                new TargetBet
                {
                    Target = 2
                },
                new TargetBet
                {
                    Target = 3
                }
            };

            Tools.CreateTestData(targets: targets, targetBets: targetBets);

            var resultTargetBets = _targetBetAction.GetCompetitionsTargetBets(competitionId: 1);

            Assert.AreEqual(2, resultTargetBets.Count);

            new List<int> { 1, 3 }
            .ForEach(id =>
            {
                Assert.IsTrue(
                    resultTargetBets.Count(targetBet => targetBet.Target == id) == 1);
            });
        }
    }
}
