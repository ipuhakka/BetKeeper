using Betkeeper.Classes;
using Api.Controllers;
using Betkeeper.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using TestTools;
using Betkeeper;
using Betkeeper.Data;

namespace Api.Test.Controllers
{
    class PageActionControllerTests
    {
        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Tools.GetTestContext();
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Sessions.RemoveRange(_context.Sessions);
            _context.SaveChanges();
        }

        [Test]
        public void Action_NotValidCredentials_ReturnsUnauthorized()
        {
            var sessions = new List<Session>
            {
                Session.GenerateSession(new Token(1))
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new PageActionController
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "InvalidToken" }
                    })
            };

            var response = controller.Action("competitions", "action") as UnauthorizedResult;

            Assert.AreEqual(401, response.StatusCode);
        }

        [TestCase("")]
        [TestCase(null)]
        public void Action_PageNullOrEmpty_ReturnsBadRequest(string invalidInput)
        {
            var controller = new PageActionController
            {
                ControllerContext = Tools.MockControllerContext()
            };


            var response = controller.Action(invalidInput, "") as BadRequestResult;
            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Action_PageNotFound_ReturnsNotFound()
        {
            var session = Session.GenerateSession(new Token(1));
            var sessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new PageActionController
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", session.Token }
                    })
            };

            var response = controller.Action("NotExistingPage", "SomeAction") as NotFoundResult;

            Assert.AreEqual(404, response.StatusCode);
        }
    }
}
