using System.Collections.Generic;
using System.Net;
using Api.Classes;
using Api.Controllers;
using NUnit.Framework;
using TestTools;

namespace Api.Test.Controllers
{
    class PageActionControllerTests
    {
        [Test]
        public void Action_NotValidCredentials_ReturnsUnauthorized()
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

            var response = controller.Action("competitions", "action");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Action_PageNullOrEmpty_ReturnsBadRequest()
        {
            var controller = new TestController()
            {
                ControllerContext = Tools.MockHttpControllerContext()
            };

            new List<string> { null, "" }.ForEach(invalidInput =>
            {
                var response = controller.Action(invalidInput, "");

                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            });
        }

        [Test]
        public void Action_PageNotFound_ReturnsNotFound()
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

            var response = controller.Action("NotExistingPage", "SomeAction");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        private class TestController : PageActionController
        {

        }
    }
}
