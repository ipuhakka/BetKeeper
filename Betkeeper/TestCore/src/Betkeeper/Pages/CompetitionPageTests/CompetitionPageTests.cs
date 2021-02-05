using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Actions;
using Betkeeper.Pages.CompetitionPage;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TestTools;

namespace Betkeeper.Test.Pages.CompetitionPageTests
{
    [TestFixture]
    public class CompetitionPageTests
    {
        private CompetitionPage _competitionPage;
        private BetkeeperDataContext _context;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _context = Tools.GetTestContext();
            _competitionPage = new CompetitionPage(new CompetitionAction());
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

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var action = new PageAction(
                1,
                "Competition",
                "DeleteCompetition",
                new Dictionary<string, object>
                {
                    { "competitionId", 1}
                });

            var response = _competitionPage.HandleAction(action);

            Assert.AreEqual(ActionResultType.Unauthorized, response.ActionResultType);
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

            Assert.AreEqual(ActionResultType.InvalidInput, response.ActionResultType);
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

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var action = new PageAction(
                1,
                "Competition",
                "DeleteCompetition",
                new Dictionary<string, object>
                {
                    { "competitionId", 1}
                });

            var response = _competitionPage.HandleAction(action);

            Assert.AreEqual(ActionResultType.OK, response.ActionResultType);
            
            Assert.AreEqual(0, _context.Competition.ToList().Count);
        }
    }
}
