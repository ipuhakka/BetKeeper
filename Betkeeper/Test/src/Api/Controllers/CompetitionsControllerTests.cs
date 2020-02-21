using System;
using System.Collections.Generic;
using System.Net;
using Api.Classes;
using Api.Controllers;
using Betkeeper;
using Betkeeper.Data;
using Betkeeper.Actions;
using NUnit.Framework;
using TestTools;

namespace Api.Test.Controllers
{

    [TestFixture]
    public class CompetitionsControllerTests
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
        public void Post_Unauthorized_Return_Unauthorized()
        {
            TokenLog.CreateToken(1);

            var controller = new TestController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "InvalidToken" }
                    })
            };

            var response = controller.Post();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Post_MissingParameters_ReturnsBadRequest()
        {
            var invalidRequests = new List<object>
            {
                new
                {
                    startTime = "Invalid datetime",
                    name = "Valid"
                },
                new
                {
                    startTime = "2019-01-01 14:40:42",
                    name = ""
                },
                new { }
            };

            var token = TokenLog.CreateToken(1);

            invalidRequests.ForEach(content =>
            {
                var controller = new TestController()
                {
                    ControllerContext = Tools.MockHttpControllerContext(
                        headers: new Dictionary<string, string>
                        {
                            { "Authorization", token.TokenString }
                        },
                        dataContent: content)
                };

                var response = controller.Post();

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            });
        }

        [Test]
        public void Post_ValidRequest_ReturnsCreated()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new TestController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    },
                    dataContent: new
                    {
                        startTime = "2019-01-01 14:45",
                        name = "testName",
                        description = "test description"
                    })
            };

            var response = controller.Post();

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var competitions = new TestCompetitionRepository().GetCompetitions();

            Assert.AreEqual(1, competitions.Count);
            Assert.AreEqual(new DateTime(2019, 1, 1, 14, 45, 0), competitions[0].StartTime);
            Assert.AreEqual("testName", competitions[0].Name);
            Assert.AreEqual("test description", competitions[0].Description);
        }

        private class TestController : CompetitionsController
        {
            public TestController()
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
