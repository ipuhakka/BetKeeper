using System.Collections.Generic;
using System.Net;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Data;
using TestTools;
using NUnit.Framework;
using System.Linq;

namespace Betkeeper.Test.Page
{
    [TestFixture]
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
        public void DeleteCompetition_UserNotCompetitionHost_ReturnsUnauthorized()
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
                    ParticipatorId = 1,
                    UserId = 1,
                    Role = (int)Enums.CompetitionRole.Participator,
                    Competition = 1
                },
                new Participator
                {
                    ParticipatorId = 2,
                    UserId = 2,
                    Role = (int)Enums.CompetitionRole.Host,
                    Competition = 1
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var action = new PageAction(
                1,
                "Competition",
                "DeleteCompetition",
                new Dictionary<string, object>
                {
                    { "competitionId", 1}
                });

            var response = new TestCompetitionPage().HandleAction(action);

            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Test]
        public void DeleteCompetition_CompetitionIdNull_ReturnsBadRequest()
        {
            var action = new PageAction(
                1,
                "Competition",
                "DeleteCompetition",
                new Dictionary<string, object>
                {
                    { "competitionId", null}
                });

            var response = new TestCompetitionPage().HandleAction(action);

            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }

        [Test]
        public void DeleteCompetition_UserIsCompetitionHost_ReturnsOK()
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
                    ParticipatorId = 1,
                    UserId = 1,
                    Role = (int)Enums.CompetitionRole.Host,
                    Competition = 1
                },
                new Participator
                {
                    ParticipatorId = 2,
                    UserId = 2,
                    Role = (int)Enums.CompetitionRole.Participator,
                    Competition = 1
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var action = new PageAction(
                1,
                "Competition",
                "DeleteCompetition",
                new Dictionary<string, object>
                {
                    { "competitionId", 1}
                });

            var response = new TestCompetitionPage().HandleAction(action);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);

            using (var context = new BetkeeperDataContext(Tools.GetTestOptionsBuilder()))
            {
                Assert.AreEqual(0, context.Competition.ToList().Count);
            }
        }

        private class TestCompetitionPage : CompetitionPage
        {
            public TestCompetitionPage()
            {
                CompetitionAction = new TestCompetitionAction();
            }
        }
        
    }
}
