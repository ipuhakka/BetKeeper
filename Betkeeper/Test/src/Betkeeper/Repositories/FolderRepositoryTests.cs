using Betkeeper.Data;
using Betkeeper.Exceptions;
using Betkeeper.Repositories;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data;

namespace Betkeeper.Test.Repositories
{
    [TestFixture]
    public class FolderRepositoryTests
    {
        [Test]
        public void GetUsersFolders_betIdNull_FormsCorrectQuery()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(MockDataTable(new List<string>()));

            new FolderRepository(database: mock.Object)
                .GetUsersFolders(userId: 1, betId: null);

            mock.Verify(database =>
                database.ExecuteQuery(
                    It.Is<string>(query =>
                        query.Contains(
                            "SELECT DISTINCT folder_name " +
                            "FROM bet_folders ")),
                    It.Is<Dictionary<string, object>>(
                        dict => (int)dict["owner"] == 1
                            && dict.Count == 1)),
                    Times.Once);
        }

        [Test]
        public void GetUsersFolders_betIdNotNull_FormsCorrectQuery()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(MockDataTable(new List<string>()));

            new FolderRepository(database: mock.Object)
                .GetUsersFolders(userId: 1, betId: 2);

            mock.Verify(database =>
                database.ExecuteQuery(
                    It.Is<string>(query =>
                        query.Contains(
                            "SELECT DISTINCT folder " +
                            "FROM  bet_in_bet_folder bf")),
                    It.Is<Dictionary<string, object>>(
                        dict => (int)dict["owner"] == 1
                            && (int)dict["betId"] == 2
                            && dict.Count == 2)),
                    Times.Once);
        }

        [Test]
        public void GetUsersFolders_NoFolders_ReturnsNone()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ExecuteQuery(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(MockDataTable(new List<string>()));

            Assert.AreEqual(0, new FolderRepository(database: mock.Object)
                .GetUsersFolders(userId: 3).Count);
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
        public void FolderHasBet_FormsValidQuery()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(true);

            var folderRepository = new FolderRepository(database: mock.Object);

            folderRepository.FolderHasBet(1, "testFolder", 2);

            mock.Verify(database =>
                database.ReadBoolean(
                    "IF EXISTS (SELECT " +
                    "* FROM bet_in_bet_folder " +
                    "WHERE owner = @userId AND folder = @folderName " +
                    "AND bet_id = @betId) " +
                    "BEGIN SELECT 1 END " +
                    "ELSE BEGIN SELECT 0 END",
                    It.Is<Dictionary<string, object>>(dict =>
                        (int)dict["userId"] == 1
                        && dict["folderName"].ToString() == "testFolder"
                        && (int)dict["betId"] == 2)),
                    Times.Once);
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
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(false);

            Assert.Throws<NotFoundException>(() =>
                new FolderRepository(database: mock.Object)
                    .DeleteFolder(3, "testFolder1"));
        }

        [Test]
        public void DeleteFolder_UserHasFolder_FolderDeleted()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(true);


            mock.Setup(database =>
                database.ExecuteCommand(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    false))
                .Returns(1);

            new FolderRepository(database: mock.Object)
                .DeleteFolder(1, "folderToDelete");

            mock.Verify(database =>
                database.ExecuteCommand(
                    "DELETE FROM bet_folders " +
                    "WHERE owner = @userId AND folder_name = @folderName",
                    It.Is<Dictionary<string, object>>(dict =>
                        (int)dict["userId"] == 1
                        && dict["folderName"].ToString() == "folderToDelete"),
                    false),
                Times.Once);
        }

        private DataTable MockDataTable(List<string> folders)
        {
            var datatable = new DataTable();

            datatable.Columns.Add(new DataColumn("folder_name"));

            foreach (var folder in folders)
            {
                var row = datatable.NewRow();

                row["folder_name"] = folder;

                datatable.Rows.Add(row);
            }

            return datatable;
        }
    }
}
