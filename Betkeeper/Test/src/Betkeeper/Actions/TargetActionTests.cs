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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";
            _context = Tools.GetTestContext();
            _targetAction = new TargetAction(
                new CompetitionRepository(_context),
                new ParticipatorRepository(_context),
                new TargetRepository(_context));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _targetAction.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Participator.RemoveRange(_context.Participator);
            _context.Target.RemoveRange(_context.Target);
            _context.Competition.RemoveRange(_context.Competition);

            _context.SaveChanges();
        }

        [Test]
        public void AddTargets_CompetitionDoesNotExist_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
                _targetAction.AddTargets(1, 1, new List<Target>()));
        }

        [Test]
        public void AddTargets_UserNotCompetitionHost_ThrowsInvalidOperationException()
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
                _context, 
                competitions: competitions,
                participators: participators);

            Assert.Throws<InvalidOperationException>(() =>
                _targetAction.AddTargets(1, 1, new List<Target>()));
        }

        [Test]
        public void AddTargets_CompetitionStarted_ThrowsInvalidOperationException()
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

            Tools.CreateTestData(_context, participators: participators, competitions: competitions);

            Assert.Throws<InvalidOperationException>(() =>
                _targetAction.AddTargets(1, 1, new List<Target>()));
        }

        [Test]
        public void AddTargets_ScoringContainsDuplicates_ThrowsArgumentException()
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

            Tools.CreateTestData(_context, participators: participators, competitions: competitions);

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
                _targetAction.AddTargets(1, 1, new List<Target> { target }));

            Assert.AreEqual("Invalid scoring arguments", exception.Message);
        }

        [Test]
        public void AddTargets_InvalidScoringsForType_ThrowsArgumentException()
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

            Tools.CreateTestData(_context, participators: participators, competitions: competitions);

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
                    _targetAction.AddTargets(1, 1, new List<Target> { target }));

                Assert.AreEqual("Invalid scoring type for target", exception.Message);
            });
        }

        [Test]
        public void AddTargets_AddsTarget()
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

            Tools.CreateTestData(_context, participators: participators, competitions: competitions);

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

            _targetAction.AddTargets(1, 1, new List<Target> { target });

            var targets = _context.Target.ToList();
            Assert.AreEqual(1, targets.Count);
            Assert.AreEqual(2, targets[0].Scoring.Count);
            Assert.AreEqual(TargetType.Result, targets[0].Type);
        }
    }
}
