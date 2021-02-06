using Betkeeper.Enums;
using Betkeeper.Actions;
using Betkeeper.Data;
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
    public class CompetitionActionTest
    {
        private CompetitionAction _action;

        private CompetitionRepository _competitionRepository;

        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Tools.GetTestContext();
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _competitionRepository = new CompetitionRepository();
            _action = new CompetitionAction();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Participator.RemoveRange(_context.Participator);
            _context.Competition.RemoveRange(_context.Competition);
            _context.SaveChanges();
        }

        [Test]
        public void GetUsersCompetitions_ReturnsUsersCompetitions()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1
                },
                new Competition
                {
                    CompetitionId = 2
                },
                new Competition
                {
                    CompetitionId = 3
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    UserId = 1,
                    Competition = 1
                },
                new Participator
                {
                    UserId = 2,
                    Competition = 3
                },
                new Participator
                {
                    UserId = 1,
                    Competition = 2
                },
                new Participator
                {
                    UserId = 2,
                    Competition = 2
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var resultCompetitions = _action.GetUsersCompetitions(1);

            Assert.AreEqual(2, resultCompetitions.Count);
            Assert.AreEqual(1, resultCompetitions[0].CompetitionId);
            Assert.AreEqual(2, resultCompetitions[1].CompetitionId);
        }

        [Test]
        public void CreateCompetition_NameInUse_ThrowsNameInUseExceptionException()
        {
            var inDatabaseCompetitions = new List<Competition>
            {
                new Competition()
                {
                    CompetitionId = 1,
                    Name = "InUseName",
                    JoinCode = "213"
                }
            };

            Tools.CreateTestData(competitions: inDatabaseCompetitions);

            var testAction = _action;

            Assert.Throws<NameInUseException>(() =>
             testAction.CreateCompetition(1, "InUseName", "Description", new DateTime(2000, 1, 1)));
        }

        [Test]
        public void CreateCompetition_CreatesParticipatorAndCompetition()
        {
            var testAction = _action;

            testAction.CreateCompetition(1, "TestName", "Description to remember", DateTime.UtcNow.AddDays(1));

            var competitions = _competitionRepository.GetCompetitions();

            Assert.AreEqual(1, competitions.Count);

            Assert.AreEqual("Description to remember", competitions[0].Description);

            Assert.AreEqual(Enums.CompetitionState.Open, competitions[0].State);
        }

        [Test]
        public void JoinCompetition_CompetitionNotFound_ThrowsNotFoundException()
        {
            Assert.Throws<ActionException>(() =>
            {
                _action.JoinCompetition("joincode", 1);
            });
        }

        [Test]
        public void JoinCompetition_CompetitionFound_ParticipatorAdded()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    JoinCode = "joincode",
                    StartTime = DateTime.Now.AddDays(1)
                },
                new Competition
                {
                    CompetitionId = 2
                }
            };

            Tools.CreateTestData(competitions: competitions);

            _action.JoinCompetition("joincode", 1);

            var participators = _context.Participator.ToList();
            Assert.AreEqual(1, participators.Count);
            Assert.AreEqual(1, participators[0].Competition);
            Assert.AreEqual(1, participators[0].UserId);
            Assert.AreEqual(Enums.CompetitionRole.Participator, participators[0].Role);
        }

        [Test]
        public void JoinCompetition_OnGoingOrFinished_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    JoinCode = "joincode1",
                    StartTime = DateTime.UtcNow.AddDays(-1)
                },
                new Competition
                {
                    CompetitionId = 2,
                    JoinCode = "joincode2",
                    StartTime = DateTime.UtcNow.AddDays(-1)
                }
            };

            Tools.CreateTestData(competitions: competitions);

            competitions.ForEach(competition =>
            {
                Assert.Throws<ActionException>(() =>
                    _action.JoinCompetition(competition.JoinCode, 1));
            });
        }

        [Test]
        public void JoinCompetition_AlreadyInCompetition_ThrowsActionException()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    JoinCode = "joincode1",
                    StartTime = DateTime.UtcNow.AddDays(1)
                }
            };

            var participators = new List<Participator>
            {
                new Participator{ Competition = 1, UserId = 1}
            };

            Tools.CreateTestData(competitions: competitions, participators: participators);

            try
            {
                _action.JoinCompetition("joincode1", 1);
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
                Assert.AreEqual("Already joined competition", e.ErrorMessage);
            }
        }

        [Test]
        public void DeleteCompetition_UserNotHost_ThrowsInvalidOperationException()
        {
            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1,
                    Role = Enums.CompetitionRole.Admin
                },
                new Participator
                {
                    Competition = 1,
                    UserId = 2,
                    Role = (int)Enums.CompetitionRole.Participator
                }
            };

            Tools.CreateTestData(participators: participators);

            participators.ForEach(participator =>
            {
                Assert.Throws<InvalidOperationException>(() =>
                    _action.DeleteCompetition(participator.UserId, 1));
            });
        }

        [Test]
        public void DeleteCompetition_UserNotSpecificCompetitionHost_ThrowsInvalidOperationException()
        {
            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 2,
                    UserId = 1,
                    Role = Enums.CompetitionRole.Host
                }
            };

            Tools.CreateTestData(participators: participators);

            participators.ForEach(participator =>
            {
                Assert.Throws<InvalidOperationException>(() =>
                    _action.DeleteCompetition(participator.UserId, 1));
            });
        }

        [Test]
        public void DeleteCompetition_UserHost_CompetitionDeleted()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1,
                    Role = Enums.CompetitionRole.Host
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var existingCompetitions = _context.Competition.ToList();
            Assert.AreEqual(1, existingCompetitions.Count);

            participators.ForEach(participator =>
            {
                _action.DeleteCompetition(participator.UserId, 1);

                existingCompetitions = _context.Competition.ToList();
                Assert.AreEqual(0, existingCompetitions.Count);
            });
        }

        [Test]
        public void GetParticipator_UserNotInCompetition_ReturnsNull()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1
                }
            };

            Tools.CreateTestData(competitions: competitions);

            Assert.IsNull(_action.GetParticipator(1, 1));
        }

        [Test]
        public void GetParticipator_ReturnsParticipator()
        {
            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    ParticipatorId = 1,
                    UserId = 1,
                    Role = (int)Enums.CompetitionRole.Participator
                }
            };

            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var participator = _action.GetParticipator(1, 1);

            Assert.AreEqual(1, participator.Competition);
            Assert.AreEqual(1, participator.ParticipatorId);
            Assert.AreEqual(1, participator.UserId);
            Assert.AreEqual(
                Enums.CompetitionRole.Participator,
                (Enums.CompetitionRole)participator.Role);
        }
    }
}
