using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betkeeper.Models;
using NUnit.Framework;

namespace Test.Models
{
    public class BetTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Tools.CreateTestDatabase();

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

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Tools.DeleteTestDatabase();
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
    }
}
