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
    }
}
