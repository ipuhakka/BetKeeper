using System.Collections.Generic;
using System.Net;
using Api.Controllers;
using Api.Classes;
using Betkeeper.Repositories;
using NUnit.Framework;
using Moq;

namespace Test.Api.Controllers
{
    [TestFixture]
    public class BetControllerTests
    {

        [Test]
        public void Get_InvalidAuthorizationToken_ReturnsUnauhtorized()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "InvalidToken" }
                    })
            };

            var response = controller.Get();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Get_CallsGetBetsWithCorrectParameters()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.GetBets(
                    It.IsAny<int>(),
                    It.IsAny<bool?>(),
                    It.IsAny<string>()));

            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _BetRepository = mock.Object
            };

            var testCases = new[]
            {
                new { Finished = (bool?)true, Folder = (string)null },
                new { Finished = (bool?)null, Folder = "testFolder"},
                new { Finished = (bool?)false, Folder = "testFolder"},
            };

            foreach (var testCase in testCases)
            {
                controller.Get(testCase.Finished, testCase.Folder);

                mock.Verify(betRepository =>
                    betRepository.GetBets(1, testCase.Finished, testCase.Folder));
            }
        }
    }
}
