using System;
using System.Collections.Generic;
using System.Net;
using Api.Controllers;
using Api.Classes;
using Betkeeper;
using Betkeeper.Repositories;
using NUnit.Framework;
using Moq;

namespace Test.Api.Controllers
{
    [TestFixture]
    public class BetsControllerTests
    {

        [Test]
        public void Get_InvalidAuthorizationToken_ReturnsUnauhtorized()
        {
            TokenLog.CreateToken(1);

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

        [Test]
        public void Post_InvalidAuthorizationToken_ReturnsUnauthorized()
        {
            TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "InvalidToken" }
                    })
            };

        var response = controller.Post();

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Post_CreateBetCalledWithCorrectParameters()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.CreateBet(
                    It.IsAny<Enums.BetResult>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<int>()));

            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        betWon = 1,
                        name = "testBet",
                        odd = 2.1,
                        stake = 2.2,
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _BetRepository = mock.Object
            };

            controller.Post();

            mock.Verify(betRepository =>
                betRepository.CreateBet(
                    Enums.BetResult.Won,
                    "testBet",
                    2.1,
                    2.2,
                    It.IsNotNull<DateTime>(),
                    1));
        }

        [Test]
        public void Post_MissingRequiredParameters_ReturnsBadRequest()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        betWon = 1,
                        name = "testBet",
                        stake = 2.2
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Post();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
