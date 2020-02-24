using System;
using System.Collections.Generic;
using System.Net;
using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Page;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Page
{
    public class CompetitionPageTests
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

        private class TestCompetitionPage : CompetitionPage
        {
            public TestCompetitionPage()
            {
                CompetitionAction = new TestAction();
            }
        }

        private class TestAction : CompetitionAction
        {
            public TestAction()
            {
                CompetitionRepository = new TestCompetitionRepository();
                ParticipatorRepository = new TestParticipatorRepository();
            }
        }
    }
}
