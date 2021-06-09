using Api.Controllers;
using Betkeeper;
using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using TestTools;

namespace Api.Test.Controllers
{
    [TestFixture]
    public class PageControllerTests
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
        public void Get_NotValidCredentials_ReturnsUnauthorized()
        {
            var sessions = new List<Session>
            {
                Session.GenerateSession(new Token(1))
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new PageController
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "InvalidToken" }
                    })
            };

            var response = controller.Get("competitions") as UnauthorizedResult;

            Assert.AreEqual(401, response.StatusCode);
        }

        [TestCase("")]
        [TestCase(null)]
        public void Get_PageNullOrEmpty_ReturnsBadRequest(string invalidInput)
        {
            var controller = new PageController
            {
                ControllerContext = Tools.MockControllerContext()
            };

            var response = controller.Get(invalidInput) as BadRequestResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Get_PageNotFound_ReturnsNotFound()
        {
            var session = Session.GenerateSession(new Token(1));
            var sessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new PageController
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", session.Token}
                    })
            };

            var response = controller.Get("NotExistingPage") as NotFoundResult;

            Assert.AreEqual(404, response.StatusCode);
        }
    }
}
