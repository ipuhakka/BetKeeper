using Api.Classes;
using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
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
            var token = TokenLog.CreateToken(1);

            var controller = new PageController
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Get("NotExistingPage") as NotFoundResult;

            Assert.AreEqual(404, response.StatusCode);
        }
    }
}
