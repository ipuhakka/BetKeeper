using System;
using System.Collections.Generic;
using Betkeeper.Data;
using Betkeeper.Models;
using Betkeeper.Exceptions;
using Betkeeper.Enums;
using TestTools;
using NUnit.Framework;
using System.Linq;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class TargetActionTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new BetkeeperDataContext(Tools.GetTestOptionsBuilder()))
            {
                context.Database.EnsureDeleted();
            }
        }

        [Test]
        public void AddTarget_CompetitionDoesNotExist_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
                new TestTargetAction().AddTarget(1, 1, new Target()));
        }

        [Test]
        public void AddTarget_UserNotInCompetition_ThrowsInvalidOperationException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1
                }
            };

            Tools.CreateTestData(competitions: competitions);

            Assert.Throws<InvalidOperationException>(() =>
                new TestTargetAction().AddTarget(1, 1, new Target()));
        }

        [Test]
        public void AddTarget_CompetitionStarted_ThrowsInvalidOperationException()
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
                new TestTargetAction().AddTarget(1, 1, new Target()));
        }

        [Test]
        public void AddTarget_ScoringContainsDuplicates_ThrowsArgumentException()
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
                    ParticipatorId = 1
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var target = new Target
            {
                Scoring = new List<Scoring>
                {
                    new Scoring
                    {
                        Points = 1,
                        Score = TargetScore.CorrectResult
                    },
                    new Scoring
                    {
                        Points = 5,
                        Score = TargetScore.CorrectResult
                    },
                }
            };

            var exception = Assert.Throws<ArgumentException>(() =>
                new TestTargetAction().AddTarget(1, 1, target));

            Assert.AreEqual("Invalid scoring arguments", exception.Message);
        }

        [Test]
        public void AddTarget_InvalidScoringsForType_ThrowsArgumentException()
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
                    ParticipatorId = 1
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            new List<Target>
            {
                new Target
                {
                    Scoring = new List<Scoring>
                    {
                        new Scoring
                        {
                            Points = 1,
                            Score = TargetScore.CorrectWinner
                        }
                    },
                    Type = TargetType.OpenQuestion
                },
                new Target
                {
                    Scoring = new List<Scoring>
                    {
                        new Scoring
                        {
                            Points = 1,
                            Score = TargetScore.CorrectWinner
                        }
                    },
                    Type = TargetType.OpenQuestion
                },
                new Target
                {
                    Scoring = new List<Scoring>
                    {
                        new Scoring
                        {
                            Points = 1,
                            Score = TargetScore.CorrectResult
                        },
                        new Scoring
                        {
                            Points = 2,
                            Score = TargetScore.CorrectResult
                        }
                    },
                    Type = TargetType.OpenQuestion
                }
            }.ForEach(target =>
            {
                var exception = Assert.Throws<ArgumentException>(() =>
                    new TestTargetAction().AddTarget(1, 1, target));

                Assert.AreEqual("Invalid scoring type for target", exception.Message);
            });
        }

        [Test]
        public void AddTarget_AddsTarget()
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
                    ParticipatorId = 1
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var target = new Target
            {
                Type = TargetType.Result,
                Scoring = new List<Scoring>
                {
                    new Scoring
                    {
                        Points = 5,
                        Score = TargetScore.CorrectResult
                    },
                    new Scoring
                    {
                        Points = 1,
                        Score = TargetScore.CorrectWinner
                    },
                }
            };

            new TestTargetAction().AddTarget(1, 1, target);

            using (var context = new BetkeeperDataContext(Tools.GetTestOptionsBuilder()))
            {
                var targets = context.Target.ToList();
                Assert.AreEqual(1, targets.Count);
                Assert.AreEqual(2, targets[0].Scoring.Count);
                Assert.AreEqual(TargetType.Result, targets[0].Type);
            }
        }
    }
}
