using Api.Classes;
using Api.Controllers;
using Betkeeper.Enums;
using Betkeeper.Exceptions;
using Betkeeper.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using TestTools;

namespace Api.Test.Controllers
{
    [TestFixture]
    public class BetsControllerTests
    {

        [Test]
        public void Get_InvalidAuthorizationToken_ReturnsUnauthorized()
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
                    It.IsAny<BetResult>(),
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
                        betResult = 1,
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
                    BetResult.Won,
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
                    It.IsAny<BetResult>(),
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
                        betResult = 1,
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
                        betResult = 1,
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

        [Test]
        public void PutBet_InvalidAuthorizationToken_ReturnsUnauthorized()
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

            var response = controller.Put(1);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void PutBet_InvalidDouble_ReturnsBadRequest()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        stake = "not double",
                        odd = 2.0,
                        name = "testName",
                        betResult = (int)BetResult.Won
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Put(1);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void PutBet_ModifyBetCalledWithCorrectParameters()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.AddBetToFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()))
                .Returns(new List<string>());

            mock.Setup(betRepository =>
                betRepository.ModifyBet(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<BetResult>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<string>()
                    ))
                .Returns(1);


            TokenLog.CreateToken(1);
            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        stake = 2.2,
                        odd = 2.0,
                        name = "testName",
                        betResult = (int)BetResult.Won
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _BetRepository = mock.Object
            };

            var response = controller.Put(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            mock.Verify(betRepository =>
                betRepository.ModifyBet(
                    1,
                    1,
                    BetResult.Won,
                    2.2,
                    2.0,
                    "testName"),
                    Times.Once);

            mock.Verify(betRepository =>
                betRepository.AddBetToFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()),
                    Times.Never);

        }

        [Test]
        public void PutBet_NullParametersIgnored()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.AddBetToFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()))
                .Returns(new List<string>());

            mock.Setup(betRepository =>
                betRepository.ModifyBet(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<BetResult>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<string>()
                    ))
                .Returns(1);

            TokenLog.CreateToken(1);
            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        stake = 2.2
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _BetRepository = mock.Object
            };

            var response = controller.Put(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            mock.Verify(betRepository =>
                betRepository.ModifyBet(
                    1,
                    1,
                    BetResult.Unresolved,
                    2.2,
                    null,
                    null),
                    Times.Once);

            mock.Verify(betRepository =>
                betRepository.AddBetToFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()),
                    Times.Never);

        }

        [Test]
        public void PutBet_ModifyBetCalledWithFolders_AddToFoldersCalled()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.AddBetToFolders(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>>()))
                .Returns(new List<string>());

            mock.Setup(betRepository =>
                betRepository.ModifyBet(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<BetResult>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<string>()
                    ))
                .Returns(1);


            TokenLog.CreateToken(1);
            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        stake = 2.2,
                        odd = 2.0,
                        name = "testName",
                        betResult = BetResult.Won,
                        folders = new List<string>
                        {
                            "folder1",
                            "folder2"
                        }
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _BetRepository = mock.Object
            };

            var response = controller.Put(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            mock.Verify(betRepository =>
                betRepository.AddBetToFolders(
                    1,
                    1,
                    new List<string>
                    {
                        "folder1",
                        "folder2"
                    }),
                    Times.Once);
        }

        [Test]
        public void PutBets_InvalidAuthorizationToken_ReturnsUnauthorized()
        {
            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "InvalidToken" }
                    })
            };

            var response = controller.Put(new List<int>());

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void PutBets_InvalidOddAndStake_ReturnsBadRequest()
        {
            var token = TokenLog.CreateToken(1);

            var invalidData = new
            {
                odd = "not double",
                stake = "not double"
            };

            var controller = new BetsController
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    },
                    dataContent: invalidData)
            };

            var response = controller.Put(new List<int>());

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// When no bets belong to user, modifybets returns 0 as nothing was modified.
        /// </summary>
        [Test]
        public void PutBets_NoBetsBelongToUser_ReturnsUnauthorized()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.GetBets(
                    It.IsAny<int>(),
                    It.IsAny<bool?>(),
                    It.IsAny<string>()))
                .Returns(new List<Betkeeper.Models.Bet>());

            mock.Setup(betRepository =>
                betRepository.ModifyBets(
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<BetResult>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<string>()
                    ))
                .Returns(0);

            var token = TokenLog.CreateToken(1);

            var data = new
            {
                betResult = 1,
                odd = 2,
                stake = 2.1
            };

            var controller = new BetsController
            {
                _BetRepository = mock.Object,
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    },
                    dataContent: data)
            };

            var response = controller.Put(new List<int> { 1, 2 });

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void PutBets_ValidRequest_ReturnsOK()
        {
            var mock = new Mock<IBetRepository>();

            mock.Setup(betRepository =>
                betRepository.GetBets(
                    It.IsAny<int>(),
                    It.IsAny<bool?>(),
                    It.IsAny<string>()))
                .Returns(new List<Betkeeper.Models.Bet>());

            mock.Setup(betRepository =>
                betRepository.ModifyBets(
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<BetResult>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<string>()
                    ))
                .Returns(1);

            var token = TokenLog.CreateToken(1);

            var data = new
            {
                betResult = 1,
                odd = 2,
                stake = 2.1
            };

            var controller = new BetsController
            {
                _BetRepository = mock.Object,
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    },
                    dataContent: data)
            };

            var response = controller.Put(new List<int> { 1, 2 });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
