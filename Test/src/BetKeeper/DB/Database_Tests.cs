using System.Collections.Generic;
using NUnit.Framework;
using BetKeeper.DB;
using BetKeeper.DB.Tables;
using BetKeeper.Exceptions;
using System.IO;

namespace Test.BetKeeper.DB
{
    [TestFixture]
    [Category("Database")]
    class Database_Tests
    {
        Database testDB = new Database();
        [OneTimeSetUp]
        public void setUp()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\BetKeeper\db"));
            testDB.ConnectionString = "Data Source = Database_testi.db; Version = 3; foreign keys=true;";
            testDB.CreateDatabase("Database_testi.db");
            testDB.CreateTables("db_schema_dump.sql");
            testDB.FillTables("db_testdata_dump.sql");
        }

        [OneTimeTearDown]
        public void tearDown()
        {
            testDB.DeleteDatabase("Database_testi.db");
        }

        [Test]
        public void test_CreateDatabase()
        {
            Database db = new Database();
            db.CreateDatabase("testi.db");
            Assert.IsTrue(File.Exists("testi.db"));
            db.DeleteDatabase("testi.db");
        }

        [Test]
        public void test_DeleteDatabase_return_1()
        {
            Database db = new Database();
            db.CreateDatabase("testidb.db");
            Assert.AreEqual(1, db.DeleteDatabase("testidb.db"));
        }

        [Test]
        public void test_DeleteDatabase_return_minus1()
        {
            Database db = new Database();
            Assert.AreEqual(-1, db.DeleteDatabase("testidb.db"));
        }

        [Test]
        public void test_CreateTables_noConnectionString()
        {
            Database db = new Database();
            db.CreateDatabase("noConnectionDb.db");
            Assert.AreEqual(-1, db.CreateTables("db_schema_dump.sql"));
            db.DeleteDatabase("noConnectionDb.db");
        }

        [Test]
        public void test_CreateTables()
        {
            Database db = new Database();
            db.ConnectionString = "Data Source = db.db; Version = 3; foreign keys=true;";
            db.CreateDatabase("db.db");
            Assert.AreEqual(1, db.CreateTables("db_schema_dump.sql"));
            db.DeleteDatabase("db.db");
        }

        [Test]
        public void test_FillTables_wrong_extension_returns_minus1()
        {
            Database db = new Database();
            db.ConnectionString = "Data Source = db.db; Version = 3; foreign keys=true;";
            db.CreateDatabase("db.db");
            db.CreateTables("db_schema_dump.sql");
            Assert.AreEqual(-1,db.FillTables("db_testdata_dump.txt"));
            db.DeleteDatabase("db.db");
        }

        [Test]
        public void test_FillTables_with_testdata_returns_1()
        {
            Database db = new Database();
            db.ConnectionString = "Data Source = db.db; Version = 3; foreign keys=true;";
            db.CreateDatabase("db.db");
            db.CreateTables("db_schema_dump.sql");
            Assert.AreEqual(1, db.FillTables("db_testdata_dump.sql"));
            db.DeleteDatabase("db.db");
        }

        [Test]
        public void test_Database_CreateBets()
        {
            testDB.CreateBet(1, "2018-08-05 14:52:40", true, 4.8, 4.7, "", new List<string>() { "valioliiga", "triplat", "tuplat" });
            Assert.AreEqual(10, Bets.GetBets(testDB.ConnectionString).Count);
            Assert.AreEqual(1, Bets.GetBetsInBetFolder(testDB.ConnectionString, "tuplat", 3).Count); //no insertion has been made to folder tuplat as user doesn't have folder named 'tuplat'.
            Assert.AreEqual(4, Bets.GetBetsInBetFolder(testDB.ConnectionString, "valioliiga", 1).Count);
            testDB.ClearTables();
            testDB.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_Database_CreateBets_throws_ArgumentException()
        {
            //Invalid datetime should produce ArgumentException
            Assert.Throws<System.ArgumentException>(() => testDB.CreateBet(1,  "2018-048-05 14:542:40", true, 4.8, 4.7, "", new List<string>() { "valioliiga", "triplat", "tuplat" }));
            testDB.ClearTables();
            testDB.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_Database_CreateBets_throws_SQLiteException()
        {
            //If user is not found, AuthenticationError is thrown.
            Assert.Throws<System.Data.SQLite.SQLiteException>(() => testDB.CreateBet(999,  "2018-08-05 14:54:40", true, 4.8, 4.7, "", new List<string>() { "valioliiga", "triplat", "tuplat" }));
            testDB.ClearTables();
            testDB.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_Database_IsPasswordCorrect_return_true()
        {
            Assert.IsTrue(testDB.IsPasswordCorrect(4, "salainensalasana"));
        }

        [Test]
        public void test_Database_IsPasswordCorrect_return_false()
        {
            Assert.IsFalse(testDB.IsPasswordCorrect(4, "wrongpassword"));
            Assert.IsFalse(testDB.IsPasswordCorrect(1, "salainensalasana"));
        }


    }
}
