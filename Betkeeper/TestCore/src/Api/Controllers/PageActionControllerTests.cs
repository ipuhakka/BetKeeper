using Api.Classes;
using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Collections.Generic;
using TestTools;

namespace Api.Test.Controllers
{
    class PageActionControllerTests
    {
        [Test]
        public void Action_NotValidCredentials_ReturnsUnauthorized()
        {
            TokenLog.CreateToken(1);

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
            var token = TokenLog.CreateToken(1);

            var controller = new PageActionController
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Action("NotExistingPage", "SomeAction") as NotFoundResult;

            Assert.AreEqual(404, response.StatusCode);
        }
    }
}
