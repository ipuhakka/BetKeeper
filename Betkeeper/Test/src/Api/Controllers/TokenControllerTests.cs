using System.Collections.Generic;
using System.Net;
using Api.Classes;
using Api.Controllers;
using Betkeeper.Data;
using Betkeeper.Models;
using NUnit.Framework;
using Moq;

namespace Test.Api.Controllers
{
    [TestFixture]
    public class TokenControllerTests
    {
        [Test]
        public void Post_AuthenticationFails_ReturnsUnauthorized()
        {
            var mock = new Mock<IUserModel>();

            mock.Setup(userModel =>
                userModel.Authenticate(It.IsAny<int>(), It.IsAny<string>())).Returns(false);

            mock.Setup(userModel =>
                userModel.GetUserId(It.IsAny<string>())).Returns(1);

            var testData = new { username = "user" };

            var controller = new TokenController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    testData, 
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    }),
                _UserModel = mock.Object
            };

            var result = controller.Post();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void Post_AuthenticationSucceeds_ReturnsOK()
        {
            var mock = new Mock<IUserModel>();

            mock.Setup(userModel =>
                userModel.Authenticate(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            mock.Setup(userModel =>
                userModel.GetUserId(It.IsAny<string>())).Returns(1);

            var testData = new { username = "user" };

            var controller = new TokenController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    testData,
                    new Dictionary<string, string>
                    {
                        { "Authorization", "fakePassword"}
                    }),
                _UserModel = mock.Object
            };

            var request = controller.Post();
            var responseBody = Http.GetHttpContent(request);

            Assert.AreEqual(HttpStatusCode.OK, request.StatusCode);
            Assert.AreEqual(12, responseBody.TokenString.ToString().Length);
            Assert.AreEqual(1, (int)responseBody.Owner);
        }

        [Test]
        public void Post_NoAuthenticationHeader_ReturnsBadRequest()
        {
            var mock = new Mock<IUserModel>();

            mock.Setup(userModel =>
                userModel.Authenticate(It.IsAny<int>(), It.IsAny<string>())).Returns(true);

            mock.Setup(userModel =>
                userModel.GetUserId(It.IsAny<string>())).Returns(1);

            var testData = new { username = "user" };

            var controller = new TokenController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    testData),
                _UserModel = mock.Object
            };

            var request = controller.Post();
            var responseBody = Http.GetHttpContent(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, request.StatusCode);
        }

        [Test]
        public void Get_TokenBelongsToUser_ReturnsOK()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new TokenController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString}
                    }),
            };

            var response = controller.Get(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Get_TokenMissing_ReturnsBadRequest()
        {
            var controller = new TokenController()
            {
                ControllerContext = Tools.MockHttpControllerContext(),
            };

            var response = controller.Get(1);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Get_TokenLogDoesNotContainToken_ReturnsNotFound()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new TokenController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "UnusedToken"}
                    }),
            };

            var response = controller.Get(1);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void Get_TokenDoesNotBelongToUser_ReturnsUnauthorized()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new TokenController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString}
                    }),
            };

            var response = controller.Get(2);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
