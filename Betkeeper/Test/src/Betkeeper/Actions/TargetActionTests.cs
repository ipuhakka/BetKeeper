using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Exceptions;
using Betkeeper.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TestTools;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class TargetActionTests
    {
        private TargetAction _targetAction;

        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
            _targetAction = new TargetAction();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Participator.RemoveRange(_context.Participator);
            _context.Target.RemoveRange(_context.Target);
            _context.Competition.RemoveRange(_context.Competition);
            _context.User.RemoveRange(_context.User);
            _context.TargetBet.RemoveRange(_context.TargetBet);

            _context.SaveChanges();
        }

        [Test]
        public void HandleTargetsUpdate_CompetitionDoesNotExist_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
                _targetAction.HandleTargetsUpdate(1, 1, new List<Target>()));
        }

        [Test]
        public void HandleTargetsUpdate_UserNotCompetitionHost_ThrowsInvalidOperationException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Today.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    ParticipatorId = 1,
                    Role = CompetitionRole.Admin,
                    Competition = 1,
                    UserId = 1
                }
            };

            Tools.CreateTestData(
                competitions: competitions,
                participators: participators);

            Assert.Throws<InvalidOperationException>(() =>
                _targetAction.HandleTargetsUpdate(1, 1, new List<Target>()));
        }

        [Test]
        public void HandleTargetsUpdate_CompetitionStarted_ThrowsInvalidOperationException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(-1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1,
                    ParticipatorId = 1
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            Assert.Throws<InvalidOperationException>(() =>
                _targetAction.HandleTargetsUpdate(1, 1, new List<Target>()));
        }

        [Test]
        public void HandleTargetsUpdate_DuplicateQuestions_ThrowsInvalidOperationException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1,
                    ParticipatorId = 1,
                    Role = CompetitionRole.Host
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var targets = new List<Target>
            {
                new Target
                {
                    ScoringDeprecated = new List<ScoringDeprecated>
                    {
                        new ScoringDeprecated
                        {
                            Points = 1,
                            Score = TargetScore.CorrectResult
                        },
                    },
                    Bet = "Bet"
                },
                new Target
                {
                    ScoringDeprecated = new List<ScoringDeprecated>
                    {
                        new ScoringDeprecated
                        {
                            Points = 1,
                            Score = TargetScore.CorrectResult
                        },
                    },
                    Bet = "Bet"
                },
            };

            var exception = Assert.Throws<ActionException>(() =>
                _targetAction.HandleTargetsUpdate(1, 1, targets));
        }

        [Test]
        public void HandleTargetsUpdate_ScoringContainsDuplicates_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1,
                    ParticipatorId = 1,
                    Role = CompetitionRole.Host
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var target = new Target
            {
                ScoringDeprecated = new List<ScoringDeprecated>
                {
                    new ScoringDeprecated
                    {
                        Points = 1,
                        Score = TargetScore.CorrectResult
                    },
                    new ScoringDeprecated
                    {
                        Points = 5,
                        Score = TargetScore.CorrectResult
                    },
                }
            };

            var exception = Assert.Throws<ActionException>(() =>
                _targetAction.HandleTargetsUpdate(1, 1, new List<Target> { target }));
        }

        [Test]
        public void HandleTargetsUpdate_InvalidScoringsForType_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1,
                    ParticipatorId = 1,
                    Role = CompetitionRole.Host
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            new List<Target>
            {
                new Target
                {
                    ScoringDeprecated = new List<ScoringDeprecated>
                    {
                        new ScoringDeprecated
                        {
                            Points = 1,
                            Score = TargetScore.CorrectWinner
                        }
                    },
                    Type = TargetType.OpenQuestion,
                    Bet = "Some bet"
                },
                new Target
                {
                    ScoringDeprecated = new List<ScoringDeprecated>
                    {
                        new ScoringDeprecated
                        {
                            Points = 1,
                            Score = TargetScore.CorrectWinner
                        }
                    },
                    Type = TargetType.OpenQuestion,
                    Bet = "Some bet"
                },
                new Target
                {
                    ScoringDeprecated = new List<ScoringDeprecated>
                    {
                        new ScoringDeprecated
                        {
                            Points = 1,
                            Score = TargetScore.CorrectResult
                        },
                        new ScoringDeprecated
                        {
                            Points = 2,
                            Score = TargetScore.CorrectResult
                        }
                    },
                    Type = TargetType.OpenQuestion,
                    Bet = "Some bet"
                }
            }.ForEach(target =>
            {
                var exception = Assert.Throws<ActionException>(() =>
                    _targetAction.HandleTargetsUpdate(1, 1, new List<Target> { target }));
            });
        }

        [Test]
        public void HandleTargetsUpdate_AddsTarget()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    StartTime = DateTime.Now.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1,
                    ParticipatorId = 1,
                    Role = CompetitionRole.Host
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var target = new Target
            {
                Type = TargetType.Result,
                ScoringDeprecated = new List<ScoringDeprecated>
                {
                    new ScoringDeprecated
                    {
                        Points = 5,
                        Score = TargetScore.CorrectResult
                    },
                    new ScoringDeprecated
                    {
                        Points = 1,
                        Score = TargetScore.CorrectWinner
                    },
                },
                Bet = "Some bet"
            };

            _targetAction.HandleTargetsUpdate(1, 1, new List<Target> { target });

            var targets = _context.Target.ToList();
            Assert.AreEqual(1, targets.Count);
            Assert.AreEqual(2, targets[0].ScoringDeprecated.Count);
            Assert.AreEqual(TargetType.Result, targets[0].Type);
        }

        [Test]
        public void CalculateCompetitionPoints_CalculatesCorrectly()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1
                }
            };

            var targets = new List<Target>
            {
                new Target
                {
                    TargetId = 1,
                    Type = TargetType.OpenQuestion,
                    Bet = "Test",
                    Result = new TargetResultItem
                    {
                        TargetBetResultDictionary = new Dictionary<int, string>
                        {
                            {1, "Correct"},
                            {2, "Wrong"}
                        }
                    },
                    ScoringDeprecated = new List<ScoringDeprecated>
                    {
                        new ScoringDeprecated
                        {
                            Points = 2,
                            Score = TargetScore.CorrectResult
                        }
                    },
                    CompetitionId = 1
                },
                new Target
                {
                    TargetId = 2,
                    Type = TargetType.Selection,
                    Result = new TargetResultItem
                    {
                        Result = "test1"
                    },
                    ScoringDeprecated = new List<ScoringDeprecated>
                    {
                        new ScoringDeprecated
                        {
                            Points = 2,
                            Score = TargetScore.CorrectResult
                        }
                    },
                    CompetitionId = 1
                },
                new Target
                {
                    TargetId = 3,
                    Type = TargetType.Result,
                    Bet = "Barca-Manu",
                    Result = new TargetResultItem
                    {
                        Result = "2-1"
                    },
                    ScoringDeprecated = new List<ScoringDeprecated>
                    {
                        new ScoringDeprecated
                        {
                            Score = TargetScore.CorrectResult,
                            Points = 2
                        },
                        new ScoringDeprecated
                        {
                            Score = TargetScore.CorrectWinner,
                            Points = 1
                        }
                    },
                    CompetitionId = 1
                }
            };

            var targetBets = new List<TargetBet>
            {
                // First bet -> correct for first user, third did not bet
                new TargetBet
                {
                    TargetBetId = 1,
                    Target = 1,
                    Participator = 1,
                    Bet = "Something"
                },
                new TargetBet
                {
                    TargetBetId = 2,
                    Target = 1,
                    Participator = 2,
                    Bet = "Something"
                },
                // Second bet, correct for first user
                new TargetBet
                {
                    TargetBetId = 3,
                    Target = 2,
                    Participator = 1,
                    Bet = "test1"
                },
                new TargetBet
                {
                    TargetBetId = 4,
                    Target = 2,
                    Participator = 2,
                    Bet = "wronganswer"
                },
                new TargetBet
                {
                    TargetBetId = 5,
                    Target = 2,
                    Participator = 3,
                    Bet = "wronganswer"
                },
                // Third bet, correct result for first, second has correct winner
                new TargetBet
                {
                    TargetBetId = 6,
                    Target = 3,
                    Participator = 1,
                    Bet = "2-1"
                },
                new TargetBet
                {
                    TargetBetId = 7,
                    Target = 3,
                    Participator = 2,
                    Bet = "3-1"
                },
                new TargetBet
                {
                    TargetBetId = 8,
                    Target = 3,
                    Participator = 3,
                    Bet = "1-1"
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    ParticipatorId = 1,
                    UserId = 1,
                    Competition = 1
                },
                new Participator
                {
                    ParticipatorId = 2,
                    UserId = 2,
                    Competition = 1
                },
                new Participator
                {
                    ParticipatorId = 3,
                    UserId = 3,
                    Competition = 1
                }
            };

            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "user1"
                },
                new User
                {
                    UserId = 2,
                    Username = "user2"
                },
                new User
                {
                    UserId = 3,
                    Username = "user3"
                }
            };

            Tools.CreateTestData(participators, competitions, users, targets, targetBets);

            var scores = _targetAction.CalculateCompetitionPoints(1);
            var points = scores.UserPointsDictionary;
            var targetItems = scores.TargetItems;

            Assert.AreEqual(6, scores.MaximumPoints);

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(6, points["user1"]);
            Assert.AreEqual(1, points["user2"]);
            Assert.AreEqual(0, points["user3"]);

            var didNotPlayTestTargetItem = targetItems[0];
            Assert.AreEqual(
                TargetResult.DidNotBet,
                didNotPlayTestTargetItem.BetItems.Single(bet => bet.User == "user3").Result);

            // Check results with last target
            var testTargetItem = targetItems[2];
            Assert.AreEqual("Barca-Manu", testTargetItem.Question);
            Assert.AreEqual(
                TargetResult.CorrectResult, 
                testTargetItem.BetItems.Single(bet => bet.User == "user1").Result);
            Assert.AreEqual(
                TargetResult.CorrectWinner,
                testTargetItem.BetItems.Single(bet => bet.User == "user2").Result);
            Assert.AreEqual(
                TargetResult.Wrong,
                testTargetItem.BetItems.Single(bet => bet.User == "user3").Result);

            Assert.AreEqual("Result: 2, winner: 1", testTargetItem.PointsAvailable);
        }
    }
}
