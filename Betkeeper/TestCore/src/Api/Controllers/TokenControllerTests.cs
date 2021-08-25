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
    public class TokenControllerTests
    {
        private BetkeeperDataContext _context;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
            Tools.InitTestSecretKey();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _context.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _context.User.RemoveRange(_context.User);
            _context.Sessions.RemoveRange(_context.Sessions);
            _context.SaveChanges();
        }

        [Test]
        public void Post_PasswordDoesNotMatch_ReturnsUnauthorized()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "user",
                    Password = Security.Encrypt("somepassword")
                }
            };

            Tools.CreateTestData(users: users);

            var testData = new { username = "user" };

            var controller = new TestController();

            controller.ControllerContext = Tools.MockControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    });

            var result = controller.Post() as UnauthorizedResult;

            Assert.AreEqual(401, result.StatusCode);
        }

        [Test]
        public void Post_UserDoesNotExist_ReturnsUnauthorized()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "user",
                    Password = "somepassword"
                }
            };

            Tools.CreateTestData(users: users);

            var testData = new { username = "notexistingusername" };

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    });

            var result = controller.Post() as UnauthorizedResult;

            Assert.AreEqual(401, result.StatusCode);
        }

        [Test]
        public void Post_AuthenticationSucceeds_ReturnsOK()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "user",
                    Password = Security.HashPlainText("somepassword", "somesalt"),
                    Salt = "somesalt"
                }
            };

            Tools.CreateTestData(users: users);

            var testData = new { username = "user" };

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "somepassword" }
                    });

            var result = controller.Post();
            var response = result as OkObjectResult;
            var token = response.Value as Token;

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(24, token.TokenString.Length);
            Assert.AreEqual(1, token.Owner);
        }

        [Test]
        public void Post_UserAlreadyHasToken_ExistingTokenReturned()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "user",
                    Password = Security.HashPlainText("fakepassword", "somesalt"),
                    Salt = "somesalt"
                }
            };

            var session = Session.GenerateSession(new Token(1));
            var sessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(users: users, sessions: sessions);

            var testData = new { username = "user" };

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakepassword"}
                    });

            var response = controller.Post() as OkObjectResult;
            var token = response.Value as Token;

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(session.Token, token.TokenString);
            Assert.AreEqual(1, token.Owner);
        }

        [Test]
        public void Post_NoAuthenticationHeader_ReturnsBadRequest()
        {
            var testData = new { username = "user" };

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(testData);

            var response = controller.Post() as BadRequestResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Get_TokenBelongsToUser_ReturnsOK()
        {
            var session = Session.GenerateSession(new Token(1));
            var sessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", session.Token }
                    });

            var response = controller.Get(1) as OkResult;

            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void Get_TokenMissing_ReturnsBadRequest()
        {
            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext();

            var response = controller.Get(1) as BadRequestResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Get_TokenLogDoesNotContainToken_ReturnsUnauthorized()
        {
            var session = Session.GenerateSession(new Token(1));
            var sessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "UnusedToken"}
                    });

            var response = controller.Get(1) as UnauthorizedResult;

            Assert.AreEqual(401, response.StatusCode);
        }

        [Test]
        public void Get_TokenDoesNotBelongToUser_ReturnsUnauthorized()
        {
            var session = Session.GenerateSession(new Token(1));
            var sessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", session.Token}
                    });

            var response = controller.Get(2) as UnauthorizedResult;

            Assert.AreEqual(401, response.StatusCode);
        }

        [Test]
        public void Delete_NoAuthorizationHeader_ReturnsBadRequest()
        {
            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext();

            var response = controller.Delete(1) as BadRequestResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Delete_TokenDoesNotBelongToUser_ReturnsNoContent()
        {
            var session1 = Session.GenerateSession(new Token(1));
            var session2 = Session.GenerateSession(new Token(2));
            var sessions = new List<Session>
            {
                session1,
                session2
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", session2.Token}
                    });

            var response = controller.Delete(1) as NoContentResult;

            Assert.AreEqual(204, response.StatusCode);
        }

        [Test]
        public void Delete_TokenBelongsToUser_ReturnsNoContent()
        {
            var session = Session.GenerateSession(new Token(1));
            var sessions = new List<Session>
            {
                session
            };

            Tools.CreateTestData(sessions: sessions);

            var controller = new TestController();
            controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", session.Token }
                    });

            var response = controller.Delete(1) as NoContentResult;

            Assert.AreEqual(204, response.StatusCode);
        }

        private class TestController : TokenController
        {
            public TestController()
            {
                UserRepository = new UserRepository();
            }
        }
    }
}
