using System;
using System.Collections.Generic;
using System.Net;
using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Models;
using Betkeeper.Page;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Page
{
    public class CompetitionsPageTests
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
                var response = new TestCompetitionPage().HandleAction(
                        new PageAction(1, "competitions", "Post", dict));

                Assert.AreEqual(
                    HttpStatusCode.BadRequest,
                    response.StatusCode);
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

            var response = new TestCompetitionPage().HandleAction(
                new PageAction(1, "competitions", "Post", parameters));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var competitions = new TestCompetitionRepository().GetCompetitions();

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

            var response = new TestCompetitionPage().HandleAction(
                new PageAction(1, "competitions", "Post", parameters));

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
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
                Assert.AreEqual(
                    HttpStatusCode.BadRequest,
                    new TestCompetitionPage().HandleAction(action).StatusCode);
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

            Assert.AreEqual(
                HttpStatusCode.NotFound, 
                new TestCompetitionPage().HandleAction(action).StatusCode);
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

            Assert.AreEqual(
                HttpStatusCode.Conflict,
                new TestCompetitionPage().HandleAction(action).StatusCode);
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

            Assert.AreEqual(
                HttpStatusCode.OK,
                new TestCompetitionPage().HandleAction(action).StatusCode);
        }

        private class TestCompetitionPage : CompetitionsPage
        {
            public TestCompetitionPage()
            {
                CompetitionAction = new TestCompetitionAction();
            }
        }
    }
}
