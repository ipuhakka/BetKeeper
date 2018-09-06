using System.IO;
using BetKeeper.DB;
using BetKeeper.DB.Tables;
using NUnit.Framework;
using System.Collections.Generic;

namespace Test.BetKeeper.DB.Tables
{
    [TestFixture]
    [Category("Database")]
    class Folders_tests
    {
        private string connectionString;
        private Database db;
        private int index = 0;
        [OneTimeSetUp]
        public void setUp()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\BetKeeper\db"));
            db = new Database();
            connectionString = "Data Source = db.db; Version = 3; foreign keys=true;";
            db.CreateDatabase("db.db");
            db.ConnectionString = connectionString;
            db.CreateTables("db_schema_dump.sql");
            db.FillTables("db_testdata_dump.sql");
        }

        [OneTimeTearDown]
        public void tearDown()
        {
            Database db = new Database();
            db.DeleteDatabase("db.db");
        }

        [Test]
        public void test_GetUsersFolders()
        {
            Assert.AreEqual(3, Folders.GetUsersFolders(connectionString, 1).Count);
        }

        [Test]
        public void test_GetUsersFolders_bet_id_1()
        {
            Assert.AreEqual(1, Folders.GetUsersFolders(connectionString, 1, 1).Count);
        }

        [Test]
        public void test_AddNewFolder_return1()
        {
            Assert.AreEqual(1, Folders.AddNewFolder(connectionString, 1, "new folder"));
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_AddNewFolder_return_minus1()
        {
            Folders.AddNewFolder(connectionString, 1, "new folder");
            Assert.AreEqual(-1, Folders.AddNewFolder(connectionString, 1, "new folder"));
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_DeleteFolder_return1()
        {
            Assert.AreEqual(1, Folders.DeleteFolder(connectionString, 1, "liiga"));
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_DeleteFolder_return0()
        {
            Assert.AreEqual(0, Folders.DeleteFolder(connectionString, 1, "notexistingfolder"));
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }
    }
}
