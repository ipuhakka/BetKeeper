using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Pages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestTools;

namespace Betkeeper.Test.Pages.CompetitionsPageTests
{
    public class CompetitionsPageTests
    {
        private CompetitionsPage _competitionsPage;
        private CompetitionRepository _competitionRepository;
        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Tools.GetTestContext();
            _competitionRepository = new CompetitionRepository();
            _competitionsPage = new CompetitionsPage(new CompetitionAction(), new CompetitionInvitationAction());
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Competition.RemoveRange(_context.Competition);
            _context.Participator.RemoveRange(_context.Participator);

            _context.SaveChanges();
        }

        [Test]
        public void Post_MissingParameters_ReturnsBadRequest()
        {
            var invalidDicts = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    { "StartTime", "Invalid datetime" },
                    { "Name", "Is valid" }
                },
                // Name cannot be empty
                new Dictionary<string, object>
                {
                    { "StartTime", "2019-01-01 14:40:42" },
                    { "Name", "" }
                },
                new Dictionary<string, object>(),
            };

            invalidDicts.ForEach(dict =>
            {
                var response = _competitionsPage.HandleAction(
                        new PageAction(1, "competitions", "Post", dict));

                Assert.AreEqual(ActionResultType.InvalidInput, response.ActionResultType);
            });
        }

        [Test]
        public void Post_ValidRequest_ReturnsCreated()
        {
            var parameters = new Dictionary<string, object>
            {
                { "StartTime", new DateTime(2019, 1, 1, 14, 45, 0) },
                { "Name", "Testi nimi" },
                { "Description", "Kuvaus" }
            };

            var response = _competitionsPage.HandleAction(
                new PageAction(1, "competitions", "Post", parameters));

            Assert.AreEqual(ActionResultType.Created, response.ActionResultType);

            var competitions = _competitionRepository.GetCompetitions();

            Assert.AreEqual(1, competitions.Count);
            Assert.AreEqual(new DateTime(2019, 1, 1, 14, 45, 0), competitions[0].StartTime);
            Assert.AreEqual("Testi nimi", competitions[0].Name);
            Assert.AreEqual("Kuvaus", competitions[0].Description);
        }

        [Test]
        public void Post_CompetitionNameAlreadyInUse_ReturnsConflict()
        {
            var inDatabaseCompetitions = new List<Competition>
            {
                new Competition()
                {
                    CompetitionId = 1,
                    Name = "Nimi",
                    JoinCode = "213"
                }
            };

            Tools.CreateTestData(competitions: inDatabaseCompetitions);

            var parameters = new Dictionary<string, object>
            {
                { "StartTime", new DateTime(2019, 1, 1, 14, 45, 0) },
                { "Name", "Nimi" },
                { "Description", "Kuvaus" }
            };

            var response = _competitionsPage.HandleAction(
                new PageAction(1, "competitions", "Post", parameters));

            Assert.AreEqual(ActionResultType.Conflict, response.ActionResultType);
        }

        [Test]
        public void JoinCompetition_EmptyJoinCode_ReturnsBadRequest()
        {
            var actions = new List<PageAction>{
                new PageAction(
                    1,
                    "Competitions",
                    "JoinCompetition",
                    new Dictionary<string, object>
                    {
                        { "JoinCode", ""}
                    }),
                new PageAction(
                    1,
                    "Competitions",
                    "JoinCompetition",
                    new Dictionary<string, object>()),
            };

            actions.ForEach(action =>
            {
                Assert.AreEqual(ActionResultType.InvalidInput, _competitionsPage.HandleAction(action).ActionResultType);
            });
        }

        [Test]
        public void JoinCompetition_CompetitionNotFound_ReturnsNotFound()
        {
            var action = new PageAction(
                1,
                "Competitions",
                "JoinCompetition",
                new Dictionary<string, object>
                {
                    { "JoinCode", "joincode"}
                });

            try
            {
                _competitionsPage.HandleAction(action);
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.NotFound, e.ActionExceptionType);
            }
        }

        [Test]
        public void JoinCompetition_CompetitionOnGoing_ReturnsConflict()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    JoinCode = "joincode",
                    StartTime = DateTime.UtcNow.AddDays(-1)
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var action = new PageAction(
            1,
            "Competitions",
            "JoinCompetition",
            new Dictionary<string, object>
            {
                { "JoinCode", "joincode"}
            });

            try
            {
                _competitionsPage.HandleAction(action);
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionResultType.Conflict, e.ActionExceptionType);
            }
        }

        [Test]
        public void JoinCompetition_CompetitionNotStarted_ReturnsOk()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1,
                    JoinCode = "joincode",
                    StartTime = DateTime.UtcNow.AddDays(1)
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var action = new PageAction(
            1,
            "Competitions",
            "JoinCompetition",
            new Dictionary<string, object>
            {
                { "JoinCode", "joincode"}
            });

            Assert.AreEqual(ActionResultType.OK, _competitionsPage.HandleAction(action).ActionResultType);
        }
    }
}
