using System;
using System.Collections.Generic;
using System.Data;
using Betkeeper.Models;
using Betkeeper.Data;
using Betkeeper.Repositories;
using Betkeeper.Exceptions;
using NUnit.Framework;
using Moq;

namespace Betkeeper.Test.Repositories
{
    public class BetRepositoryTests
    {
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
        public void DeleteFromFolders_ReturnsFoldersWhereBetWas()
        {
            var folderMock = new Mock<IFolderRepository>();

            folderMock.Setup(folderRepo =>
                folderRepo.UserHasFolder(
                    1,
                    It.IsAny<string>()))
                .Returns(true);

            folderMock.Setup(folderRepo =>
                folderRepo.FolderHasBet(
                    1,
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns((int userId, string folderName, int betId) =>
                {
                    if (folderName == "betNotInFolder")
                    {
                        return false;
                    }

                    return true;
                });

            var databaseMock = new Mock<IDatabase>();

            databaseMock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(1);

            var deleteFromFolders = new List<string>
            {
                "delete1",
                "betNotInFolder",
                "delete2"
            };

            var deletedFrom = new BetRepository(
                database: databaseMock.Object,
                folderRepository: folderMock.Object)
                    .DeleteBetFromFolders(1, 1, deleteFromFolders);

            Assert.AreEqual(2, deletedFrom.Count);
            Assert.AreEqual("delete1", deletedFrom[0]);
            Assert.AreEqual("delete2", deletedFrom[1]);
        }

        [Test]
        public void DeleteFromFolder_DoesNotDeleteBetFromOtherUsersFolder()
        {
            var folderMock = new Mock<IFolderRepository>();

            folderMock.Setup(folderRepo =>
                folderRepo.UserHasFolder(
                    1,
                    It.IsAny<string>()))
                .Returns((int userId, string folderName) =>
                {
                    if (folderName == "notUsersFolder")
                    {
                        return false;
                    }

                    return true;
                });

            folderMock.Setup(folderRepo =>
                folderRepo.FolderHasBet(
                    1,
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(true);

            var databaseMock = new Mock<IDatabase>();

            databaseMock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(1);

            var deleteFromFolders = new List<string>
            {
                "delete1",
                "notUsersFolder",
                "delete2",
            };

            var deletedFrom = new BetRepository(
                database: databaseMock.Object,
                folderRepository: folderMock.Object)
                .DeleteBetFromFolders(1, 1, deleteFromFolders);

            Assert.AreEqual(2, deletedFrom.Count);
            Assert.AreEqual("delete1", deletedFrom[0]);
            Assert.AreEqual("delete2", deletedFrom[1]);
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
            var folderMock = new Mock<IFolderRepository>();

            folderMock.Setup(folderRepo =>
                folderRepo.UserHasFolder(
                    1,
                    It.IsAny<string>()))
                .Returns((int userId, string folderName) =>
                {
                    if (folderName == "notUsersFolder")
                    {
                        return false;
                    }

                    return true;
                });

            var databaseMock = new Mock<IDatabase>();

            databaseMock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(1);

            var betRepository = new BetRepository(
                database: databaseMock.Object,
                folderRepository: folderMock.Object);

            var testFolders = new List<string>
            {
                "testFolder1",
                "testFolder2",
                "notUsersFolder"
            };

            var addedToFolders = betRepository.AddBetToFolders(1, 1, testFolders);

            Assert.AreEqual(2, addedToFolders.Count);
            Assert.AreEqual("testFolder1", addedToFolders[0]);
            Assert.AreEqual("testFolder2", addedToFolders[1]);
        }

        [Test]
        public void AddBetToFolders_AddsBetOnlyToFoldersWhereItIsNot()
        {
            var folderMock = new Mock<IFolderRepository>();

            folderMock.Setup(folderRepo =>
                folderRepo.UserHasFolder(
                    1,
                    It.IsAny<string>()))
                .Returns(true);

            folderMock.Setup(folderRepo =>
                folderRepo.FolderHasBet(
                    1,
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns((int userId, string folderName, int betId) =>
                {
                    // Bet already in folder
                    if (folderName == "alreadyContainsBet") 
                    {
                        return true;
                    }

                    return false;
                });

            var databaseMock = new Mock<IDatabase>();

            databaseMock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(1);

            var betRepository = new BetRepository(
                database: databaseMock.Object,
                folderRepository: folderMock.Object);
            
            var testFolders = new List<string>
            {
                "testFolder1",
                "testFolder2",
                "alreadyContainsBet"
            };

            var addedToFolders = betRepository.AddBetToFolders(1, 1, testFolders);

            Assert.AreEqual(2, addedToFolders.Count);
            Assert.AreEqual("testFolder1", addedToFolders[0]);
            Assert.AreEqual("testFolder2", addedToFolders[1]);
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
                    Times.Exactly(2),
                    "Odd parameter update called unexpected amount");

            mock.Verify(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.Is<Dictionary<string, object>>(parameters =>
                        !parameters.ContainsKey("stake")),
                    false),
                    Times.Exactly(2),
                    "Stake parameter update called unexpected amount");

            mock.Verify(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.Is<Dictionary<string, object>>(parameters =>
                        !parameters.ContainsKey("name")),
                    false),
                    Times.Exactly(2),
                    "Name parameter update called unexpected amount");
        }

        /// <summary>
        /// Test should return 2, as one bet is not found and two are.
        /// </summary>
        [Test]
        public void ModifyBets_ReturnsUpdatedBetCount()
        {
            var mock = new Mock<IDatabase>();

            var mockDataTable = MockDataTable(
                new List<Bet>
                {
                    new Bet(null, "testi", 2, 2, new DateTime(), 1)
                    {
                        BetId = 2
                    },
                    new Bet(null, "testi", 2, 2, new DateTime(), 1)
                    {
                        BetId = 3
                    }
                });

            mock.Setup(database =>
                database.ExecuteQuery(
                    "SELECT * FROM bets WHERE owner = @userId",
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(mockDataTable);

            mock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(2);

            var betRepository = new BetRepository(database: mock.Object);

            var modifiedBets = betRepository.ModifyBets(
                new List<int> { 1, 2, 3 },
                1,
                Enums.BetResult.Won);

            mock.Verify(database => database.ExecuteCommand(
                It.Is<string>(query => query.Contains("IN (@intValue2, @intValue3)")),
                It.Is<Dictionary<string, object>>(dict =>
                    (int)dict["intValue2"] == 2
                    && (int)dict["intValue3"] == 3),
                false),
                Times.Once);

            Assert.AreEqual(2, modifiedBets);
        }

        [Test]
        public void ModifyBets_GetBetsReturns0Bets_ExecuteCommandNotCalled()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    "SELECT * FROM bets WHERE owner = @userId",
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(MockDataTable(new List<Bet>()));

            mock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(2);

            var betRepository = new BetRepository(database: mock.Object);

            var modifiedBets = betRepository.ModifyBets(
                new List<int> { 1, 2, 3 },
                1,
                Enums.BetResult.Won);

            mock.Verify(database => database.ExecuteCommand(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                false),
                Times.Never);

            Assert.AreEqual(0, modifiedBets);
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
