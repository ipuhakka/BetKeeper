using System;
using System.Collections.Generic;
using Betkeeper;
using Betkeeper.Models;
using Betkeeper.Exceptions;
using NUnit.Framework;

namespace Test.Betkeeper.Models
{
    public class BetModelTests
    {
        BetModel _BetModel;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _BetModel = new BetModel();
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
                "INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) " +
                    "VALUES ('testiveto', 2, 4, datetime('now', 'localTime'), 1, 1, 3);" +
                 "INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) " +
                    "VALUES ('testiveto', 4, 5, datetime('now', 'localTime'), 1, 1, 4);" +
                "INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) " +
                    "VALUES ('testiveto', 2.64, 3, datetime('now', 'localTime'), 1, -1, 5);" +
                "INSERT OR REPLACE INTO bets(name, odd, bet, date_time, owner, bet_won, bet_id) " +
                    "VALUES(NULL, 3.13, 3, datetime('now', 'localTime'), 2, 0, 2); " +
                "INSERT OR REPLACE INTO bet_folders VALUES('testFolder1', 1);" +
                "INSERT OR REPLACE INTO bet_folders VALUES('testFolder2', 1);" +
                "INSERT OR REPLACE INTO bet_folders VALUES('someTestFolder', 2);" +
                "INSERT OR REPLACE INTO bet_in_bet_folder VALUES('testFolder1', 1, 1);" +
                "INSERT OR REPLACE INTO bet_in_bet_folder VALUES('someTestFolder', 2, 2);" +
                "INSERT OR REPLACE INTO bet_in_bet_folder VALUES('testFolder2', 3, 2);";

            Tools.ExecuteNonQuery(setUpCommand);
        }

        [TearDown]
        public void TearDown()
        {
            Tools.ClearTables(new List<string>
            {
                "bets",
                "bet_in_bet_folder",
                "bet_folders",
                "users"
            });
        }

        [Test]
        public void GetBet_BetIdOfAnotherOwner_ReturnsNull()
        {
            Assert.IsNull(_BetModel.GetBet(2, 1));
        }

        [Test]
        public void GetBet_OwnerHasBet_ReturnsCorrectData()
        {
            var bet = _BetModel.GetBet(1, 1);

            Assert.AreEqual("testiveto", bet.Name);
            Assert.AreEqual(2.64, bet.Odd);
            Assert.AreEqual(3, bet.Stake);
            Assert.AreEqual(1, bet.BetId);
        }

        [Test]
        public void GetBets_UserIdGiven_ReturnsUsersBets()
        {
            Assert.AreEqual(4, _BetModel.GetBets(userId: 1).Count);
        }

        [Test]
        public void GetBets_NoParameters_ReturnsAllBets()
        {
            Assert.AreEqual(5, _BetModel.GetBets().Count);
        }

        [Test]
        public void GetBets_WhereBetFinishedAndUserId_ReturnsUsersFinishedBets()
        {
            Assert.AreEqual(3, _BetModel.GetBets(userId: 1, betFinished: true).Count);
        }

        [Test]
        public void GetBets_WhereBetFinished_ReturnsAllFinishedBets()
        {
            Assert.AreEqual(4, _BetModel.GetBets(betFinished: true).Count);
        }

        [Test]
        public void GetBets_WhereBetUnfinished_ReturnsAllUnfinishedBets()
        {
            Assert.AreEqual(1, _BetModel.GetBets(betFinished: false).Count);
        }

        [Test]
        public void GetBets_WhereFolder_ReturnsOnlyFoldersBets()
        {
            var bets = _BetModel.GetBets(1, null, "testFolder1");

            Assert.AreEqual(1, bets.Count);
        }

        [Test]
        public void GetBets_WhereFolderAndFinishedTrue_ReturnsFoldersFinishedBets()
        {
            var bets = _BetModel.GetBets(1, true, "testFolder1");

            Assert.AreEqual(1, bets.Count);
        }

        [Test]
        public void GetBets_WhereFolderAndFinishedFalse_ReturnsFoldersUnfinishedBets()
        {
            var bets = _BetModel.GetBets(1, false, "testFolder1");

            Assert.AreEqual(0, bets.Count);
        }

        [Test]
        public void DeleteBet_BetNotUsers_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
                _BetModel.DeleteBet(2, 1));
        }

        [Test]
        public void DeleteBet_UsersBet_Returns1BetDeleted()
        {
            Assert.AreEqual(1, _BetModel.DeleteBet(1, 1));
            Assert.IsNull(_BetModel.GetBet(1, 1));
        }

        [Test]
        public void DeleteFromFolders_ReturnsFoldersWhereBetIs()
        {
            var deleteFromFolders = new List<string>
            {
                "testFolder1",
                "testFolder2"
            };

            var deletedFrom = _BetModel.DeleteBetFromFolders(1, 1, deleteFromFolders);

            Assert.AreEqual(1, deletedFrom.Count);
            Assert.AreEqual("testFolder1", deletedFrom[0]);
        }

        [Test]
        public void DeleteFromFolder_DoesNotDeleteBetFromOtherUsersFolder()
        {
            var deleteFromFolders = new List<string>
            {
                "someTestFolder"
            };

            var deletedFrom = _BetModel.DeleteBetFromFolders(1, 1, deleteFromFolders);

            Assert.AreEqual(0, deletedFrom.Count);
        }

        [Test]
        public void CreateBet_UserDoesNotExist_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
             _BetModel.CreateBet(
                betWon: true,
                name: "testName",
                odd: 2.5,
                stake: 2.2,
                playedDate: new DateTime(2019, 1, 1, 14, 25, 12),
                userId: 999));
        }

        [Test]
        public void CreateBet_OnSuccess_BetAdded()
        {
            Assert.AreEqual(1, _BetModel.CreateBet(
                betWon: true,
                name: "testName",
                odd: 2.5,
                stake: 2.2,
                playedDate: new DateTime(2019, 1, 1, 14, 25, 12),
                userId: 1));

            var addedBet = _BetModel.GetBet(6, 1);

            Assert.AreEqual(Enums.BetResult.Won, addedBet.BetResult);
            Assert.AreEqual(new DateTime(2019, 1, 1, 14, 25, 12), addedBet.PlayedDate);
            Assert.AreEqual(2.5, addedBet.Odd);
            Assert.AreEqual(2.2, addedBet.Stake);
            Assert.AreEqual("testName", addedBet.Name);
            Assert.AreEqual(1, addedBet.Owner);
        }

        [Test]
        public void AddBetToFolders_AddsBetOnlyToUsersFolders()
        {
            var testFolders = new List<string>
            {
                "testFolder2",
                "someTestFolder"
            };

            var addedToFolders = _BetModel.AddBetToFolders(1, 1, testFolders);

            Assert.AreEqual(1, addedToFolders.Count);
            Assert.AreEqual("testFolder2", addedToFolders[0]);
        }

        [Test]
        public void AddBetToFolders_AddsBetOnlyToFoldersWhereItIsNot()
        {
            var testFolders = new List<string>
            {
                "testFolder1",
                "testFolder2"
            };

            var addedToFolders = _BetModel.AddBetToFolders(1, 1, testFolders);

            Assert.AreEqual(1, addedToFolders.Count);
            Assert.AreEqual("testFolder2", addedToFolders[0]);
        }

        [Test]
        public void ModifyBet_UserDoesNotHaveBet_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
                _BetModel.ModifyBet(2, 1, betWon: true));
        }

        [Test]
        public void ModifyBet_ModifiesInputtedParameters()
        {
            _BetModel.ModifyBet(
                betId: 1,
                userId: 1,
                betWon: true,
                stake: 5.2,
                odd: 1.2,
                name: "modifyTest");

            var modifiedBet = _BetModel.GetBet(1, 1);

            Assert.AreEqual(modifiedBet.Stake, 5.2);
            Assert.AreEqual(modifiedBet.Odd, 1.2);
            Assert.AreEqual(modifiedBet.BetResult, Enums.BetResult.Won);
            Assert.AreEqual(modifiedBet.Name, "modifyTest");
        }

        [Test]
        public void ModifyBet_DoesNotModifyNullParameters()
        {
            _BetModel.ModifyBet(
                betId: 1,
                userId: 1,
                betWon: false);

            var modifiedBet = _BetModel.GetBet(1, 1);

            Assert.AreEqual(modifiedBet.Stake, 3);
            Assert.AreEqual(modifiedBet.Odd, 2.64);
            Assert.AreEqual(modifiedBet.BetResult, Enums.BetResult.Lost);
            Assert.AreEqual(modifiedBet.Name, "testiveto");
        }

        [Test]
        public void ModifyBet_BetWonAlwaysModified()
        {
            var betWonDict = new Dictionary<int, bool?>
            {
                { 1, true},
                { 0, false},
                { -1, null}
            };
            
            foreach(var betResult in betWonDict)
            {
                _BetModel.ModifyBet(
                    betId: 1,
                    userId: 1,
                    betWon: betResult.Value);

                var modifiedBet = _BetModel.GetBet(1, 1);

                Assert.AreEqual(modifiedBet.BetResult, (Enums.BetResult)betResult.Key);
            }
        }
    }
}
