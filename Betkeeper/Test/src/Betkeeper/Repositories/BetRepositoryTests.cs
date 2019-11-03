using System;
using System.Collections.Generic;
using System.Data;
using Betkeeper.Models;
using Betkeeper.Data;
using Betkeeper.Repositories;
using Betkeeper.Exceptions;
using NUnit.Framework;
using Moq;
using TestTools;

namespace Betkeeper.Test.Repositories
{
    public class BetRepositoryTests
    {
        BetRepository _BetRepository;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _BetRepository = new BetRepository();
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
        public void GetBet_NoBetsFound_ReturnsNull()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    "SELECT * FROM bets WHERE bet_id = @betId AND owner = @userId",
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            var betRepository = new BetRepository(database: mock.Object);

            Assert.IsNull(betRepository.GetBet(1, 1));
        }

        [Test]
        public void GetBet_ReturnsFirstBetInDataTable()
        {
            var mock = new Mock<IDatabase>();

            var mockDataTable = MockDataTable(
                new List<Bet>
                {
                    new Bet(true, "testi", 2, 2, new DateTime(), 1),
                    new Bet(false, "testi2", 3, 3, new DateTime(), 1)
                });

            mock.Setup(database =>
                database.ExecuteQuery(
                    "SELECT * FROM bets WHERE bet_id = @betId AND owner = @userId",
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(mockDataTable);

            var betRepository = new BetRepository(database: mock.Object);

            var resultBet = betRepository.GetBet(1, 1);

            Assert.AreEqual("testi", resultBet.Name);
            Assert.AreEqual(Enums.BetResult.Won, resultBet.BetResult);
        }

        [Test]
        public void GetBets_FolderNull_SelectAllBetsQueryCalled()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            var betRepository = new BetRepository(database: mock.Object);

            betRepository.GetBets(
                userId: 1,
                betFinished: null, 
                folder: null);

            mock.Verify(database => 
                database.ExecuteQuery(
                    It.Is<string>(
                        query => query.Contains("SELECT * FROM bets")),
                    It.IsAny<Dictionary<string, object>>()), 
                Times.Once);
        }

        [Test]
        public void GetBets_BetFinishedQueryValid()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            var betRepository = new BetRepository(database: mock.Object);

            betRepository.GetBets(
                userId: 1,
                betFinished: true,
                folder: null);

            betRepository.GetBets(
                userId: 1,
                betFinished: false,
                folder: null);

            betRepository.GetBets(
                userId: 1,
                betFinished: null,
                folder: null);

            mock.Verify(database =>
                database.ExecuteQuery(
                    It.Is<string>(
                        query => query.Contains("bet_won != @betFinished")),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);

            mock.Verify(database =>
                database.ExecuteQuery(
                    It.Is<string>(
                        query => query.Contains("bet_won = @betFinished")),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);

            mock.Verify(database =>
                database.ExecuteQuery(
                    It.Is<string>(
                        query => !query.Contains("bet_won")),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that user id is used correctly.
        /// GetBetsFromFolder is not called if userId is null.
        /// </summary>
        [Test]
        public void GetBets_UserIdQueryValid()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            var betRepository = new BetRepository(database: mock.Object);

            betRepository.GetBets(
                userId: 1,
                betFinished: null,
                folder: null);

            betRepository.GetBets(
                userId: null,
                betFinished: null,
                folder: null);

            betRepository.GetBets(
                userId: null,
                betFinished: null,
                folder: "not null");

            betRepository.GetBets(
                userId: 1,
                betFinished: null,
                folder: "not null");

            mock.Verify(database =>
                database.ExecuteQuery(
                    It.Is<string>(
                        query => query.Contains("owner = @userId")),
                    It.Is<Dictionary<string, object>>(
                        dictionary => dictionary.ContainsKey("userId")
                            && (int)dictionary["userId"] == 1)),
                Times.Exactly(2),
                "With user id called incorrect amount");

            mock.Verify(database =>
                database.ExecuteQuery(
                    It.Is<string>(
                        query => !query.Contains("owner = @userId")),
                    It.Is<Dictionary<string, object>>(
                        dictionary => !dictionary.ContainsKey("userId"))),
                Times.Exactly(2),
                "Without user id called incorrect amount");
        }

        [Test]
        public void GetBets_FolderNotNull_GetBetsFromFoldersQueryCalled()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            var betRepository = new BetRepository(database: mock.Object);

            betRepository.GetBets(
                userId: 1,
                betFinished: null,
                folder: "folder");

            mock.Verify(database =>
                database.ExecuteQuery(
                    It.Is<string>(
                        query => query.Contains("SELECT * FROM bet_in_bet_folder")),
                    It.IsAny<Dictionary<string, object>>()),
                Times.Once);
        }

        [Test]
        public void DeleteBet_BetNotFound_ThrowsNotFoundException()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    "SELECT * FROM bets WHERE bet_id = @betId AND owner = @userId",
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            Assert.Throws<NotFoundException>(() =>
                new BetRepository(database: mock.Object).DeleteBet(2, 1));
        }

        [Test]
        public void DeleteBet_BetFound_DeleteQueryCalled()
        {
            var mock = new Mock<IDatabase>();

            var mockDataTable = MockDataTable(
                new List<Bet>
                {
                    new Bet(true, "testi", 2, 2, new DateTime(), 1)
                });

            mock.Setup(database =>
                database.ExecuteQuery(
                    "SELECT * FROM bets WHERE bet_id = @betId AND owner = @userId",
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(mockDataTable);

            mock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<bool>()))
                .Returns(1);

            var betRepository = new BetRepository(database: mock.Object);

            betRepository.DeleteBet(1, 2);

            mock.Verify(database =>
                database.ExecuteCommand(
                    "DELETE FROM bets WHERE bet_id = @betId",
                    new Dictionary<string, object>{
                        {"betId", 1}
                    },
                    false),
                    Times.Once);
        }

        [Test]
        public void DeleteFromFolders_ReturnsFoldersWhereBetIs()
        {
            // TODO: FIX
            var deleteFromFolders = new List<string>
            {
                "testFolder1",
                "testFolder2"
            };

            var deletedFrom = _BetRepository.DeleteBetFromFolders(1, 1, deleteFromFolders);

            Assert.AreEqual(1, deletedFrom.Count);
            Assert.AreEqual("testFolder1", deletedFrom[0]);
        }

        [Test]
        public void DeleteFromFolder_DoesNotDeleteBetFromOtherUsersFolder()
        {
            // TODO: FIX
            var deleteFromFolders = new List<string>
            {
                "someTestFolder"
            };

            var deletedFrom = _BetRepository.DeleteBetFromFolders(1, 1, deleteFromFolders);

            Assert.AreEqual(0, deletedFrom.Count);
        }

        [Test]
        public void CreateBet_UserDoesNotExist_ThrowsNotFoundException()
        {
            var mock = new Mock<IUserRepository>();

            mock.Setup(userRepo =>
                userRepo.UserIdExists(
                    1))
                .Returns(false);

            var betRepository = new BetRepository(mock.Object);

            Assert.Throws<NotFoundException>(() =>
             betRepository.CreateBet(
                betResult: Enums.BetResult.Unresolved,
                name: "testName",
                odd: 2.5,
                stake: 2.2,
                playedDate: new DateTime(2019, 1, 1, 14, 25, 12),
                userId: 999));
        }

        [Test]
        public void CreateBet_OnSuccess_BetAddedReturnsLastInsertedId()
        {
            var userRepoMock = new Mock<IUserRepository>();

            userRepoMock.Setup(userRepo =>
                userRepo.UserIdExists(
                    3))
                .Returns(true);

            var databaseMock = new Mock<IDatabase>();

            databaseMock.Setup(database =>
                database.ExecuteCommand(It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<bool>()))
                .Returns(1);

            var betRepository = new BetRepository(
                userRepoMock.Object, 
                databaseMock.Object);

            betRepository.CreateBet(
                Enums.BetResult.Unresolved,
                "testi",
                2.8,
                2.4,
                new DateTime(2019, 1, 1),
                3);

            databaseMock.Verify(database =>
                database.ExecuteCommand(
                    "INSERT INTO bets " +
                "(bet_won, name, odd, bet, date_time, owner) " +
                "VALUES (@betResult, @name, @odd, @bet, @dateTime, @owner);",
                new Dictionary<string, object>
                {
                    {"betResult", (int)Enums.BetResult.Unresolved },
                    {"name", "testi" },
                    {"odd", 2.8 },
                    {"bet", 2.4 },
                    {"dateTime", new DateTime(2019, 1, 1) },
                    {"owner", 3 }
                },
                true),
                Times.Once);
        }

        [Test]
        public void AddBetToFolders_AddsBetOnlyToUsersFolders()
        {
            // TODO: FIX
            var testFolders = new List<string>
            {
                "testFolder2",
                "someTestFolder"
            };

            var addedToFolders = _BetRepository.AddBetToFolders(1, 1, testFolders);

            Assert.AreEqual(1, addedToFolders.Count);
            Assert.AreEqual("testFolder2", addedToFolders[0]);
        }

        [Test]
        public void AddBetToFolders_AddsBetOnlyToFoldersWhereItIsNot()
        {
            // TODO: FIX
            var testFolders = new List<string>
            {
                "testFolder1",
                "testFolder2"
            };

            var addedToFolders = _BetRepository.AddBetToFolders(1, 1, testFolders);

            Assert.AreEqual(1, addedToFolders.Count);
            Assert.AreEqual("testFolder2", addedToFolders[0]);
        }

        [Test]
        public void ModifyBet_UserDoesNotHaveBet_ThrowsNotFoundException()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    "SELECT * FROM bets WHERE bet_id = @betId AND owner = @userId",
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(new DataTable());

            Assert.Throws<NotFoundException>(() =>
                new BetRepository(database: mock.Object)
                .ModifyBet(2, 1, Enums.BetResult.Won));
        }

        [Test]
        public void ModifyBet_QueryDoesNotIncludeNullParameters()
        {
            var mock = new Mock<IDatabase>();

            var mockDataTable = MockDataTable(
                new List<Bet>
                {
                    new Bet(true, "testi", 2, 2, new DateTime(), 1)
                });

            mock.Setup(database =>
                database.ExecuteQuery(
                    "SELECT * FROM bets WHERE bet_id = @betId AND owner = @userId",
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(mockDataTable);

            mock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(1);

            var betRepository = new BetRepository(database: mock.Object);

            betRepository.ModifyBet(
                1, 
                1, 
                Enums.BetResult.Lost, 
                stake: 2.1);

            betRepository.ModifyBet(
                1,
                1,
                Enums.BetResult.Lost,
                odd: 2.1);

            betRepository.ModifyBet(
                1,
                1,
                Enums.BetResult.Lost,
                name: "modified");

            mock.Verify(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.Is<Dictionary<string, object>>(parameters =>
                        !parameters.ContainsKey("odd")),
                    false),
                    Times.Exactly(2));

            mock.Verify(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.Is<Dictionary<string, object>>(parameters =>
                        !parameters.ContainsKey("stake")),
                    false),
                    Times.Exactly(2));

            mock.Verify(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.Is<Dictionary<string, object>>(parameters =>
                        !parameters.ContainsKey("name")),
                    false),
                    Times.Exactly(2));
        }

        private DataTable MockDataTable(List<Bet> bets)
        {
            var datatable = new DataTable();

            datatable.Columns.Add(new DataColumn("bet_won"));
            datatable.Columns.Add(new DataColumn("bet"));
            datatable.Columns.Add(new DataColumn("odd"));
            datatable.Columns.Add(new DataColumn("date_time"));
            datatable.Columns.Add(new DataColumn("bet_id"));
            datatable.Columns.Add(new DataColumn("owner"));
            datatable.Columns.Add(new DataColumn("name"));

            foreach (var bet in bets)
            {
                var row = datatable.NewRow();

                row["bet_won"] = (int)bet.BetResult;
                row["bet"] = bet.Stake;
                row["odd"] = bet.Odd;
                row["name"] = bet.Name;
                row["date_time"] = bet.PlayedDate;
                row["owner"] = bet.Owner;
                row["bet_id"] = bet.BetId;

                datatable.Rows.Add(row);
            }

            return datatable;
        }
    }
}
