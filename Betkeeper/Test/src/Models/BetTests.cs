using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betkeeper;
using Betkeeper.Models;
using Betkeeper.Exceptions;
using NUnit.Framework;

namespace Test.Models
{
    public class BetTests
    {

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Tools.CreateTestDatabase();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Tools.DeleteTestDatabase();
        }

        [SetUp]
        public void Setup()
        {
            var setUpCommand =
                "INSERT OR REPLACE INTO users(username, password, user_id) " +
                    "VALUES ('testi', 'salasana', 1);" +
                "INSERT OR REPLACE INTO users(username, password, user_id) " +
                    "VALUES('käyttäjä2', 'salasana2', 2);" +
                "INSERT OR REPLACE INTO users(username, password, user_id) " +
                    "VALUES('käyttäjä3', 'salasana3', 3);" +
                "INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) " +
                    "VALUES ('testiveto', 2.64, 3, datetime('now', 'localTime'), 1, 0, 1);" +
                "INSERT OR REPLACE INTO bets(name, odd, bet, date_time, owner, bet_won, bet_id) " +
                    "VALUES(NULL, 3.13, 3, datetime('now', 'localTime'), 2, 0, 2); " +
                "INSERT OR REPLACE INTO bet_in_bet_folder VALUES('testFolder1', 1, 1);" +
                "INSERT OR REPLACE INTO bet_in_bet_folder VALUES('someTestFolder', 2, 2);" +
                "INSERT OR REPLACE INTO bet_in_bet_folder VALUES('testFolder2', 1, 1);";

            Tools.ExecuteNonQuery(setUpCommand);
        }

        [TearDown]
        public void TearDown()
        {
            Tools.ClearTables(new List<string>
            {
                "bets",
                "bet_in_bet_folder",
                "users"
            });
        }

        [Test]
        public void GetBet_BetIdOfAnotherOwner_ReturnsNull()
        {
            Assert.IsNull(Bet.GetBet(2, 1));
        }

        [Test]
        public void GetBet_OwnerHasBet_ReturnsCorrectData()
        {
            var bet = Bet.GetBet(1, 1);

            Assert.AreEqual("testiveto", bet.Name);
            Assert.AreEqual(2.64, bet.Odd);
            Assert.AreEqual(3, bet.Stake);
            Assert.AreEqual(1, bet.BetId);
        }

        [Test]
        public void DeleteBet_BetNotUsers_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
                Bet.DeleteBet(2, 1));
        }

        [Test]
        public void DeleteBet_UsersBet_Returns1BetDeleted()
        {
            Assert.AreEqual(1, Bet.DeleteBet(1, 1));
            Assert.IsNull(Bet.GetBet(1, 1));
        }

        [Test]
        public void CreateBet_UserDoesNotExist_ThrowsNotFoundException()
        {
            var bet = new Bet(
                betWon: true,
                name: "testName",
                odd: 2.5,
                stake: 2.2,
                playedDate: new DateTime(2019, 1, 1, 14, 25, 12),
                userId: 999);

            Assert.Throws<NotFoundException>(() =>
             bet.CreateBet());
        }

        [Test]
        public void CreateBet_OnSuccess_BetAdded()
        {
            var bet = new Bet(
                betWon: true,
                name: "testName",
                odd: 2.5,
                stake: 2.2,
                playedDate: new DateTime(2019, 1, 1, 14, 25, 12),
                userId: 1);

            Assert.AreEqual(1, bet.CreateBet());

            var addedBet = Bet.GetBet(3, 1);

            Assert.AreEqual(Enums.BetResult.Won, addedBet.BetResult);
            Assert.AreEqual(new DateTime(2019, 1, 1, 14, 25, 12), addedBet.PlayedDate);
            Assert.AreEqual(2.5, addedBet.Odd);
            Assert.AreEqual(2.2, addedBet.Stake);
            Assert.AreEqual("testName", addedBet.Name);
            Assert.AreEqual(1, addedBet.Owner);
        }
    }
}
