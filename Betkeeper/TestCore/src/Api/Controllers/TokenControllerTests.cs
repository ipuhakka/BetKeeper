using Api.Classes;
using Api.Controllers;
using Betkeeper;
using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using TestTools;

namespace Api.Test.Controllers
{
    [TestFixture]
    public class TokenControllerTests
    {
        private BetkeeperDataContext _context;

        private TestController _controller;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
            _controller = new TestController();
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
            TokenLog.ClearTokenLog();
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

            _controller.ControllerContext = Tools.MockControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    });

            var result = _controller.Post() as UnauthorizedResult;

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

            _controller.ControllerContext = Tools.MockControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    });

            var result = _controller.Post() as UnauthorizedResult;

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
                    Password = Security.Encrypt("somepassword")
                }
            };

            Tools.CreateTestData(users: users);

            var testData = new { username = "user" };

            _controller.ControllerContext = Tools.MockControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "somepassword" }
                    });

            var result = _controller.Post();
            var response = result as OkObjectResult;
            var token = response.Value as Token;

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(12, token.TokenString.Length);
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
                    Password = Security.Encrypt("fakePassword")
                }
            };

            Tools.CreateTestData(users: users);

            var testToken = TokenLog.CreateToken(1);

            var testData = new { username = "user" };

            _controller.ControllerContext = Tools.MockControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    });

            var response = _controller.Post() as OkObjectResult;
            var token = response.Value as Token;

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(testToken.TokenString, token.TokenString);
            Assert.AreEqual(1, token.Owner);
        }

        [Test]
        public void Post_NoAuthenticationHeader_ReturnsBadRequest()
        {
            var testData = new { username = "user" };

            _controller.ControllerContext = Tools.MockControllerContext(testData);

            var response = _controller.Post() as BadRequestResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Get_TokenBelongsToUser_ReturnsOK()
        {
            var token = TokenLog.CreateToken(1);

            _controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString}
                    });

            var response = _controller.Get(1) as OkResult;

            Assert.AreEqual(200, response.StatusCode);
        }

        [Test]
        public void Get_TokenMissing_ReturnsBadRequest()
        {
            _controller.ControllerContext = Tools.MockControllerContext();

            var response = _controller.Get(1) as BadRequestResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Get_TokenLogDoesNotContainToken_ReturnsNotFound()
        {
            TokenLog.CreateToken(1);

            _controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "UnusedToken"}
                    });

            var response = _controller.Get(1) as NotFoundResult;

            Assert.AreEqual(404, response.StatusCode);
        }

        [Test]
        public void Get_TokenDoesNotBelongToUser_ReturnsUnauthorized()
        {
            var token = TokenLog.CreateToken(1);

            _controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString}
                    });

            var response = _controller.Get(2) as UnauthorizedResult;

            Assert.AreEqual(401, response.StatusCode);
        }

        [Test]
        public void Delete_NoAuthorizationHeader_ReturnsBadRequest()
        {
            _controller.ControllerContext = Tools.MockControllerContext();

            var response = _controller.Delete(1) as BadRequestResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Delete_TokenDoesNotBelongToUser_ReturnsUnauthorized()
        {
            TokenLog.CreateToken(1);
            var token2 = TokenLog.CreateToken(2);

            _controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token2.TokenString}
                    });

            var response = _controller.Delete(1) as UnauthorizedResult;

            Assert.AreEqual(401, response.StatusCode);
        }

        [Test]
        public void Delete_TokenBelongsToUser_ReturnsNoContent()
        {
            var token = TokenLog.CreateToken(1);

            _controller.ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString}
                    });

            var response = _controller.Delete(1) as NoContentResult;

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
