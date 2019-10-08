using System;
using System.Collections.Generic;
using System.Net;
using Api.Controllers;
using Api.Classes;
using Betkeeper;
using Betkeeper.Repositories;
using Betkeeper.Exceptions;
using Betkeeper.Models;
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

            mock.Setup(betRepository =>
                betRepository.AddBetToFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()));

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

            mock.Verify(betRepository =>
                betRepository.AddBetToFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()), Times.Never);
        }

        [Test]
        public void Post_AddBetToFolders_CalledWithCorrectParameters()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.CreateBet(
                    It.IsAny<Enums.BetResult>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<int>())).Returns(1);

            mock.Setup(betRepository =>
                betRepository.AddBetToFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()));

            var token = TokenLog.CreateToken(1);

            var testFolders = new List<string>
            {
                "folder1",
                "folder2"
            };

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        betWon = 1,
                        name = "testBet",
                        odd = 2.1,
                        stake = 2.2,
                        folders = testFolders
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _BetRepository = mock.Object
            };

            controller.Post();

            mock.Verify(betRepository =>
                betRepository.AddBetToFolders(
                    1,
                    1,
                    testFolders));
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

        [Test]
        public void Delete_InvalidAuthorizationToken_ReturnsUnauthorized()
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

            var response = controller.Delete(1);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Delete_NoFoldersThrowsNotFoundException_ReturnsNotFound()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.DeleteBet(
                    It.IsAny<int>(),
                    It.IsAny<int>()
                    )).Throws(new NotFoundException("testError"));

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

            var response = controller.Delete(1);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Tests than when bet is deleted from no folders successfully, 
        /// not found is returned.
        /// </summary>
        [Test]
        public void Delete_DeleteFromFoldersReturnsEmptyList_ReturnsNotFound()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.DeleteBetFromFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()
                    )).Returns(new List<string>());

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

            var response = controller.Delete(1, new List<string>
            {
                "testFolderToDelete"
            });

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void Delete_DeleteFromFoldersReturnsList_ReturnsOK()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.DeleteBetFromFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()
                    )).Returns(new List<string> { "testFolderToDelete" });

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

            var response = controller.Delete(1, new List<string>
            {
                "testFolderToDelete"
            });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Delete_DeleteDoesNotThrowException_ReturnsNoContent()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.DeleteBet(
                    It.IsAny<int>(),
                    It.IsAny<int>()
                    ));

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

            var response = controller.Delete(1);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
