using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper;
using Betkeeper.Data;
using Betkeeper.Models;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class BetTests
    {
        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            _context = Tools.GetTestContext();
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Bet.RemoveRange(_context.Bet);
            _context.BetInBetFolder.RemoveRange(_context.BetInBetFolder);
            _context.SaveChanges();
        }

        [Test]
        public void GetBet_UserDoesNotHaveBet_ReturnsNull()
        {
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 2
                }
            };

            Tools.CreateTestData(bets: bets);

            Assert.IsNull(new BetRepository().GetBet(1, 1));
        }

        [Test]
        public void GetBet_UserHasBet_ReturnsBet()
        {
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 1,
                    Name = "test",
                    BetResult = Enums.BetResult.Lost
                }
            };

            Tools.CreateTestData(bets: bets);

            var bet = new BetRepository().GetBet(1, 1);

            Assert.AreEqual("test", bet.Name);
            Assert.AreEqual(Enums.BetResult.Lost, bet.BetResult);
        }

        [Test]
        public void GetBets_ReturnsCorrectBets()
        {
            var bets = new List<Bet>
            {
                new Bet
                {
                    BetId = 1,
                    Owner = 1,
                    Name = "test1",
                    BetResult = Enums.BetResult.Lost
                },
                new Bet
                {
                    BetId = 2,
                    Owner = 1,
                    Name = "test2",
                    BetResult = Enums.BetResult.Won
                },
                new Bet
                {
                    BetId = 3,
                    Owner = 1,
                    Name = "test3",
                    BetResult = Enums.BetResult.Unresolved
                }
            };

            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "testFolder",
                    Owner = 1
                },
                new BetInBetFolder
                {
                    BetId = 3,
                    FolderName = "testFolder",
                    Owner = 1
                }
            };

            Tools.CreateTestData(
                betInBetFolders: betInBetFolders,
                bets: bets);

            var results = new BetRepository().GetBets(
                1,
                Enums.BetResult.Unresolved,
                "testFolder");

            Assert.AreEqual(1, results.Count);

            Assert.AreEqual("test3", results[0].Name);
        }
    }
}
