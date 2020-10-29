using Api.Classes;
using Api.Controllers;
using Betkeeper;
using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Enums;
using Betkeeper.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TestTools;

namespace Api.Test.Controllers
{
    [TestFixture]
    public class BetsControllerTests
    {
        BetkeeperDataContext _context { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
        }

        [TearDown]
        public void TearDown()
        {
            _context.User.RemoveRange(_context.User);
            _context.Bet.RemoveRange(_context.Bet);
            _context.BetInBetFolder.RemoveRange(_context.BetInBetFolder);
            _context.Folder.RemoveRange(_context.Folder);

            _context.SaveChanges();
        }

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
            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    FolderName = "test",
                    BetId = 2,
                    Owner = 1
                }
            };

            var bets = new List<Bet>
            {
                new Bet
                {
                    Owner = 1,
                    BetId = 1,
                    BetResult = BetResult.Unresolved
                },
                new Bet
                {
                    BetResult = BetResult.Lost,
                    Owner = 1,
                    BetId = 2
                },
                new Bet
                {
                    BetResult = BetResult.Won,
                    Owner = 1,
                    BetId = 3
                }
            };

            Tools.CreateTestData(
                bets: bets,
                betInBetFolders: betInBetFolders);

            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Get(finished: true, folder: "test");

            var results = JsonConvert.DeserializeObject<List<Bet>>(Http.GetHttpContent(response).ToString());

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(BetResult.Lost, results[0].BetResult);
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
        public void Post_CreateBet_BetCreated()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1
                }
            };

            Tools.CreateTestData(users: users);

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
                    })
            };

            controller.Post();

            var bets = _context.Bet.ToList();

            Assert.AreEqual(1, bets.Count);

            Assert.AreEqual(BetResult.Won, bets[0].BetResult);
            Assert.AreEqual(2.1, bets[0].Odd);
            Assert.AreEqual(2.2, bets[0].Stake);
        }

        [Test]
        public void Post_AddBetToFolders_BetAddedToFolders()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                },
                new Folder
                {
                    FolderName = "test",
                    Owner = 2
                },
                new Folder
                {
                    FolderName = "test2",
                    Owner = 1
                }
            };

            var users = new List<User>
            {
                new User
                {
                    UserId = 1
                }
            };

            Tools.CreateTestData(folders: folders, users: users);

            var token = TokenLog.CreateToken(1);

            var testFolders = new List<string>
            {
                "test",
                "test2"
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
                    })
            };

            controller.Post();

            Assert.AreEqual(2, _context.BetInBetFolder.Count());
            Assert.AreEqual(1, _context.Bet.Count());
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
            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Delete(1);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        /// Tests than when bet is deleted from no folders successfully, 
        /// not found is returned.
        /// </summary>
        [Test]
        public void Delete_DeleteFromFoldersNothingDeleted_ReturnsNotFound()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
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
            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    FolderName = "testFolderToDelete",
                    Owner = 1,
                    BetId = 1
                }
            };

            Tools.CreateTestData(betInBetFolders: betInBetFolders);

            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
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
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 1
                }
            };

            Tools.CreateTestData(bets: bets);

            var token = TokenLog.CreateToken(1);

            var controller = new BetsController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Delete(1);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            Assert.AreEqual(0, _context.Bet.Count());
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
        public void PutBet_ModifyBet_ModifiesBet()
        {
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 1,
                    BetResult = BetResult.Unresolved,
                    Name = "test"
                }
            };

            Tools.CreateTestData(bets: bets);

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
                    })
            };

            var response = controller.Put(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var results = _context.Bet.ToList();

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual("testName", results[0].Name);
            Assert.AreEqual(BetResult.Won, results[0].BetResult);
        }

        [Test]
        public void PutBet_NullParametersIgnored()
        {
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 1,
                    Stake = 5,
                    Odd = 2.1,
                    Name = "test",
                    PlayedDate = new System.DateTime(2000, 1, 1),
                    BetResult = BetResult.Unresolved
                }
            };

            Tools.CreateTestData(bets: bets);

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
                    })
            };

            var response = controller.Put(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var bet = _context.Bet.Single(betEntity => betEntity.BetId == 1);

            Assert.AreEqual("test", bet.Name);
            Assert.AreEqual(new DateTime(2000, 1, 1), bet.PlayedDate);
            Assert.AreEqual(2.1, bet.Odd);
            Assert.AreEqual(BetResult.Unresolved, bet.BetResult);
            Assert.AreEqual(2.2, bet.Stake);
        }

        [Test]
        public void PutBet_ModifyBetCalledWithFolders_AddToFoldersCalled()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "folder1",
                    Owner = 1
                },
                new Folder
                {
                    FolderName = "folder2",
                    Owner = 1
                }
            };

            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders, bets: bets);

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
                    })
            };

            var response = controller.Put(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var betInBetFolders = _context.BetInBetFolder.ToList();

            Assert.AreEqual(2, betInBetFolders.Count);
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
        public void PutBets_NoBetsBelongToUser_ReturnsNotFound()
        {
            var token = TokenLog.CreateToken(1);

            var data = new
            {
                betResult = 1,
                odd = 2,
                stake = 2.1
            };

            var controller = new BetsController
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    },
                    dataContent: data)
            };

            var response = controller.Put(new List<int> { 1, 2 });

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void PutBets_ValidRequest_UpdatesAllProperties()
        {
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 1,
                    Stake = 5,
                    Odd = 2.1,
                    Name = "test",
                    PlayedDate = new DateTime(2000, 1, 1),
                    BetResult = BetResult.Unresolved
                },
                new Bet
                {
                    BetId = 2,
                    Owner = 1,
                    Stake = 5,
                    Odd = 2.1,
                    Name = "test",
                    PlayedDate = new DateTime(2000, 1, 1),
                    BetResult = BetResult.Unresolved
                },
            };

            Tools.CreateTestData(bets: bets);

            var token = TokenLog.CreateToken(1);

            var data = new
            {
                betResult = 1,
                odd = 2,
                stake = 2.1,
                name = "test2"
            };

            var controller = new BetsController
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    },
                    dataContent: data)
            };

            var response = controller.Put(new List<int> { 1, 2 });

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var results = _context.Bet.ToList();

            Assert.AreEqual(2, results.Count);

            results.ForEach(updatedBet =>
            {
                Assert.AreEqual(2, updatedBet.Odd);
                Assert.AreEqual(2.1, updatedBet.Stake);
                Assert.AreEqual("test2", updatedBet.Name);
                Assert.AreEqual(BetResult.Won, updatedBet.BetResult);
            });
        }
    }
}
