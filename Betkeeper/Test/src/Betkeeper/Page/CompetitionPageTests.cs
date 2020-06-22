using Betkeeper.Data;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Pages;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TestTools;

namespace Betkeeper.Test.Pages
{
    [TestFixture]
    public class CompetitionPageTests
    {
        private CompetitionPage _competitionPage;
        private BetkeeperDataContext _context;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";
            _context = Tools.GetTestContext();
            _competitionPage = new CompetitionPage(
                new Betkeeper.Actions.CompetitionAction(
                    new CompetitionRepository(_context), new ParticipatorRepository(_context)));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Competition.RemoveRange(_context.Competition);
            _context.Participator.RemoveRange(_context.Participator);

            _context.SaveChanges();
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
                    Role = Enums.CompetitionRole.Host,
                    Competition = 1
                }
            };

            Tools.CreateTestData(_context, participators: participators, competitions: competitions);

            var action = new PageAction(
                1,
                "Competition",
                "DeleteCompetition",
                new Dictionary<string, object>
                {
                    { "competitionId", 1}
                });

            var response = _competitionPage.HandleAction(action);

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

            var response = _competitionPage.HandleAction(action);

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
                    Role = Enums.CompetitionRole.Host,
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

            Tools.CreateTestData(_context, participators: participators, competitions: competitions);

            var action = new PageAction(
                1,
                "Competition",
                "DeleteCompetition",
                new Dictionary<string, object>
                {
                    { "competitionId", 1}
                });

            var response = _competitionPage.HandleAction(action);

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode);
            
            Assert.AreEqual(0, _context.Competition.ToList().Count);
        }
    }
}
