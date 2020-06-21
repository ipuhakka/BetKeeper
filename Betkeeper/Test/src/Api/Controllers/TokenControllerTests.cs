using Api.Classes;
using Api.Controllers;
using Betkeeper;
using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Models;
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
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";
            _context = new BetkeeperDataContext(Tools.GetTestOptionsBuilder());
            _controller = new TestController(_context);
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
                    Password = "somepassword"
                }
            };

            Tools.CreateTestData(_context, users: users);

            var testData = new { username = "user" };

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    });

            var result = _controller.Post();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
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

            Tools.CreateTestData(_context, users: users);

            var testData = new { username = "notexistingusername" };

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    });

            var result = _controller.Post();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
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
                    Password = "somepassword"
                }
            };

            Tools.CreateTestData(_context, users: users);

            var testData = new { username = "user" };

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "somepassword" }
                    });

            var request = _controller.Post();
            var responseBody = Http.GetHttpContent(request);

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.AreEqual(12, responseBody.tokenString.ToString().Length);
            Assert.AreEqual(1, (int)responseBody.owner);
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
                    Password = "fakePassword"
                }
            };

            Tools.CreateTestData(_context, users: users);

            var testToken = TokenLog.CreateToken(1);

            var testData = new { username = "user" };

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    });

            var request = _controller.Post();
            var responseBody = Http.GetHttpContent(request);

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.AreEqual(testToken.TokenString, responseBody.tokenString.ToString());
            Assert.AreEqual(1, (int)responseBody.owner);
        }

        [Test]
        public void Post_NoAuthenticationHeader_ReturnsBadRequest()
        {
            var testData = new { username = "user" };

            _controller.ControllerContext = Tools.MockHttpControllerContext(testData);

            var response = _controller.Post();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Get_TokenBelongsToUser_ReturnsOK()
        {
            var token = TokenLog.CreateToken(1);

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString}
                    });

            var response = _controller.Get(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Get_TokenMissing_ReturnsBadRequest()
        {
            _controller.ControllerContext = Tools.MockHttpControllerContext();

            var response = _controller.Get(1);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Get_TokenLogDoesNotContainToken_ReturnsNotFound()
        {
            TokenLog.CreateToken(1);

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "UnusedToken"}
                    });

            var response = _controller.Get(1);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void Get_TokenDoesNotBelongToUser_ReturnsUnauthorized()
        {
            var token = TokenLog.CreateToken(1);

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString}
                    });

            var response = _controller.Get(2);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Delete_NoAuthorizationHeader_ReturnsBadRequest()
        {
            _controller.ControllerContext = Tools.MockHttpControllerContext();

            var response = _controller.Delete(1);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Delete_TokenDoesNotBelongToUser_ReturnsUnauthorized()
        {
            TokenLog.CreateToken(1);
            var token2 = TokenLog.CreateToken(2);

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token2.TokenString}
                    });

            var response = _controller.Delete(1);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Delete_TokenBelongsToUser_ReturnsNoContent()
        {
            var token = TokenLog.CreateToken(1);

            _controller.ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString}
                    });

            var response = _controller.Delete(1);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        private class TestController : TokenController
        {
            public TestController(BetkeeperDataContext context)
            {
                UserRepository = new UserRepository(context);
            }
        }
    }
}
