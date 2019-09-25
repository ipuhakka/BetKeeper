using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using Api.Controllers;
using Betkeeper.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using Moq;
using System.Net;

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

            var request = new HttpRequestMessage();

            request.Content = new StringContent(JsonConvert.SerializeObject(testData));

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "BASIC", "fakePassword");

            var controller = new TokenController()
            {
                ControllerContext = new HttpControllerContext
                {
                    Request = request
                },
                _UserModel = mock.Object
            };

            var result = controller.Post();

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
        }
    }
}
