using Api.Classes;
using Api.Controllers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using TestTools;

namespace Api.Test.Controllers
{
    [TestFixture]
    public class PageControllerTests
    {
        [Test]
        public void Get_NotValidCredentials_ReturnsUnauthorized()
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

            var response = controller.Get("competitions");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Get_PageNullOrEmpty_ReturnsBadRequest()
        {
            var controller = new TestController()
            {
                ControllerContext = Tools.MockHttpControllerContext()
            };

            new List<string> { null, "" }.ForEach(invalidInput =>
            {
                var response = controller.Get(invalidInput);

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            });
        }

        [Test]
        public void Get_PageNotFound_ReturnsNotFound()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new TestController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Get("NotExistingPage");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        private class TestController : PageController
        {

        }
    }
}
