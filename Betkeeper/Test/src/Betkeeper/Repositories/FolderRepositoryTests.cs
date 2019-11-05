using System.Collections.Generic;
using Betkeeper.Data;
using Betkeeper.Repositories;
using Betkeeper.Exceptions;
using NUnit.Framework;
using Moq;
using TestTools;

namespace Betkeeper.Test.Repositories
{
    [TestFixture]
    public class FolderRepositoryTests
    {
        FolderRepository _FolderRepository;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _FolderRepository = new FolderRepository();

            Tools.CreateTestDatabase();

            var setUpCommand =
                "INSERT OR REPLACE INTO users(username, password, user_id) " +
                    "VALUES ('testi', 'salasana', 1);" +
                "INSERT OR REPLACE INTO users(username, password, user_id) " +
                    "VALUES('käyttäjä2', 'salasana2', 2);" +
                "INSERT OR REPLACE INTO users(username, password, user_id) " +
                    "VALUES('käyttäjä3', 'salasana3', 3);" +
                "INSERT OR REPLACE INTO bet_folders VALUES('testFolder1', 1);" +
                "INSERT OR REPLACE INTO bet_folders VALUES('testFolder2', 1);" +
                "INSERT OR REPLACE INTO bet_folders VALUES('testFolder3', 2);" +
                "INSERT OR REPLACE INTO bet_folders VALUES('testFolder4', 1);" +
                "INSERT OR REPLACE INTO bets (name, odd, bet, date_time, owner, bet_won, bet_id) " +
                    "VALUES (NULL, 2.64, 3, datetime('now', 'localTime'), 1, 0, 1);" +
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
        public void GetUsersFolders_ReturnsUsersFolders()
        {
            var expectedFolderList = new List<string>
            {
                "testFolder1",
                "testFolder2",
                "testFolder4"
            };

            var folders = _FolderRepository.GetUsersFolders(userId: 1);

            Assert.AreEqual(3, folders.Count);

            folders.ForEach(folder =>
            {
                Assert.IsTrue(expectedFolderList.Contains(folder));
            });
        }

        [Test]
        public void GetUsersFolders_WhereBet_ReturnsCorrectResults()
        {
            var expectedFolderList = new List<string>
            {
                "testFolder1",
                "testFolder2"
            };

            var folders = _FolderRepository.GetUsersFolders(userId: 1, betId: 1);

            Assert.AreEqual(2, folders.Count);

            folders.ForEach(folder =>
            {
                Assert.IsTrue(expectedFolderList.Contains(folder));
            });
        }

        [Test]
        public void GetUsersFolders_WhereBet_BetDoesNotBelongToUser_ReturnsNone()
        {
            var folders = _FolderRepository.GetUsersFolders(userId: 1, betId: 2);

            Assert.AreEqual(0, folders.Count);
        }

        [Test]
        public void GetUsersFolders_NoFolders_ReturnsNone()
        {
            Assert.AreEqual(0, _FolderRepository.GetUsersFolders(userId: 3).Count);
        }

        [Test]
        public void UserHasFolder_QueriedWithValidParameters()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(true);

            var folderRepository = new FolderRepository(database: mock.Object);

            folderRepository.UserHasFolder(1, "testFolder");

            mock.Verify(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.Is<Dictionary<string, object>>(dict =>
                        (int)dict["userId"] == 1
                        && dict["folderName"].ToString() == "testFolder")),
                    Times.Once);
        }

        [Test]
        public void FolderHasBet_ReturnsTrue()
        {
            Assert.IsTrue(_FolderRepository.FolderHasBet(userId: 1, folderName: "testFolder1", betId: 1));
        }

        [Test]
        public void FolderHasBet_BetNotInFolder_ReturnsFalse()
        {
            Assert.IsFalse(_FolderRepository.FolderHasBet(userId: 1, folderName: "testFolder1", betId: 2));
        }

        [Test]
        public void AddNewFolder_FolderExists_ThrowsFolderExistsException()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(true);

            var folderRepository = new FolderRepository(database: mock.Object);

            Assert.Throws<FolderExistsException>(() =>
                folderRepository.AddNewFolder(1, "testFolder1"));
        }

        [Test]
        public void AddNewFolder_UserDoesNotHaveFolder_InsertQueryCalled()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(false);

            mock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(1);

            var folderRepository = new FolderRepository(database: mock.Object);

            folderRepository.AddNewFolder(3, "testFolder1");

            mock.Verify(database =>
                database.ExecuteCommand(
                    It.Is<string>(query => query.Contains(
                        "INSERT INTO bet_folders")),
                    It.Is<Dictionary<string, object>>(dict =>
                        dict["folder"].ToString() == "testFolder1"
                        && (int)dict["userId"] == 3),
                    false),
                    Times.Once);
        }

        [Test]
        public void DeleteFolder_UserDoesNotHaveFolder_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
                _FolderRepository.DeleteFolder(3, "testFolder1"));
        }

        [Test]
        public void DeleteFolder_UserHasFolder_FolderDeleted()
        {
            var userId = 3;
            var folderName = "testFolder1";

            _FolderRepository.AddNewFolder(userId, folderName);
            Assert.AreEqual(1, _FolderRepository.DeleteFolder(userId, folderName));
        }
    }
}
