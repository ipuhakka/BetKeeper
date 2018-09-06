using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using BetKeeper.DB;
using BetKeeper.DB.Tables;
using BetKeeper;
using BetKeeper.Exceptions;

namespace Test.BetKeeper.DB.Tables
{
    [TestFixture]
    [Category("Database")]
    class Bets_tests
    {
        private string connectionString;
        private Database db;

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
        public void test_GetBets_searchAll()
        {   /* testdata dump contains 9 bets. */
            List<Bet> bets = Bets.GetBets(connectionString);
            Assert.AreEqual(9, bets.Count);
        }

        [Test]
        public void test_GetBets_search_jannu27()
        {
            List<Bet> bets = Bets.GetBets(connectionString, 1);
            Assert.AreEqual(4, bets.Count);

            foreach(Bet b in bets)
            {
                Assert.AreEqual(1, b.getOwner());
            }
        }

        [Test]
        public void test_GetBets_search_jannu27_bets_finished()
        {
            List<Bet> bets = Bets.GetBets(connectionString, 1, true);
            Assert.AreEqual(3, bets.Count);
        }

        [Test]
        public void test_GetBet_return_minus1()
        {
            /*Not used bet_id returns 0*/
            Assert.IsNull(Bets.GetBet(connectionString, 19));
        }

        [Test]
        public void test_GetBet_return_correct_bet()
        {
            Assert.AreEqual(2, Bets.GetBet(connectionString, 2).getId());
        }

        [Test]
        public void test_GetBetsInBetFolder()
        {
            Assert.AreEqual(3, Bets.GetBetsInBetFolder(connectionString, "valioliiga", 1).Count);
        }

        [Test]
        public void test_GetBetsInBetFolder_WhereFinished_true()
        {
            Assert.AreEqual(2, Bets.GetBetsInBetFolder(connectionString, "valioliiga", 1, true).Count);
        }

        [Test]
        public void test_GetBetsInBetFolder_WhereFinished_false()
        {
            Assert.AreEqual(1, Bets.GetBetsInBetFolder(connectionString, "valioliiga", 1, false).Count);
        }

        [Test]
        public void test_CreateBet()
        {
            Assert.AreEqual(10, Bets.CreateBet(connectionString, 1, "2018-08-05 14:52:40", 4.8, 4.7, "", false));
            List<Bet> bets = Bets.GetBets(connectionString, 1);
            Assert.AreEqual(5, bets.Count);
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_CreateBet_invalid_datetime_throws_ArgumentException()
        {
            Assert.Throws<System.ArgumentException>(() => Bets.CreateBet(connectionString, 1, "2018-084-05 14:524:40", 4.8, 4.7, "", true));
        }

        [Test]
        public void test_CreateBet_JavaScriptDate()
        {
            //JavaScript string format of Date()-object should be able to be parsed correctly.
            Assert.DoesNotThrow(() => Bets.CreateBet(connectionString, 1, "2018-08-31T08:47:25.236Z", 4.8, 4.7, "", true));
        }

        [Test]
        public void test_CreateBet_throws_SQLiteException()
        {
            /*When user is not found in the database SQLiteException should be thrown.*/
            Assert.Throws<System.Data.SQLite.SQLiteException>(() => Bets.CreateBet(connectionString, 999, "2018-08-05 14:52:40", 4.8, 4.7, "", true));
        }
        
        [Test]
        public void test_AddBetInBetFolders()
        {
            int id = Bets.CreateBet(connectionString, 1, "2018-08-05 14:52:40", 4.8, 4.7, "", false);
            Assert.AreEqual(2, Bets.AddBetToFolders(connectionString, new List<string>() { "valioliiga", "liiga" }, id, 1));
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_AddBetInBetFolders_wrongUsersFolder()
        {
            //folders that do not belong to user should be ignored.
            int id = Bets.CreateBet(connectionString, 1, "2018-08-05 14:52:40", 4.8, 4.7, "", false);
            Bets.AddBetToFolders(connectionString, new List<string>() { "tuplat", "valioliiga" }, id, 1);
            Assert.AreEqual(4, Bets.GetBetsInBetFolder(connectionString, "valioliiga", 1).Count); //check no insert has been made.
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_AddBetInBetFolders_unknownBet()
        {
            int unused_bet_id = 900;
            Bets.CreateBet(connectionString, 1, "2018-08-05 14:52:40", 4.8, 4.7, "", false);
            Assert.Throws<UnknownBetError>(() => Bets.AddBetToFolders(connectionString, new List<string>() { "tuplat", "valioliiga" }, unused_bet_id, 1)); 
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_ModifyBet_return_1()
        {
            Assert.AreEqual(1, Bets.ModifyBet(connectionString, 5, 1, true, 100, 2, ""));
        }

        [Test]
        public void test_ModifyBet_throw_UnknownBetError()
        {
            Assert.Throws<UnknownBetError>(() => Bets.ModifyBet(connectionString, 19, 1, true, 100, 2, ""));
        }

        [Test]
        public void test_ModifyBet_throw_AuthenticationError()
        {
            Assert.Throws<AuthenticationError>(() => Bets.ModifyBet(connectionString, 4, 1, true, 100, 2, ""));
        }

        [Test]
        public void test_DeleteBet_return1()
        {
            Assert.AreEqual(1, Bets.DeleteBet(connectionString, 5, 1));
            Assert.AreEqual(8, Bets.GetBets(connectionString).Count);
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_DeleteBet_throws_AuthenticationError()
        {
            Assert.Throws<AuthenticationError>(() => Bets.DeleteBet(connectionString, 7, 1)); //User trying to delete another user's bet.
            Assert.AreEqual(9, Bets.GetBets(connectionString).Count); //check nothing was deleted
        }

        [Test]
        public void test_DeleteBet_throws_UnknownError()
        {
            Assert.Throws<UnknownBetError>(() => Bets.DeleteBet(connectionString, 12, 1)); //Unexisting bet
            Assert.AreEqual(9, Bets.GetBets(connectionString).Count); //check nothing was deleted
        }

        [Test]
        public void test_DeleteBetFromFoldersreturn1()
        {
            Assert.AreEqual(1, Bets.DeleteBetFromFolders(connectionString, 1, 1, new List<string>() { "liiga", "valioliiga" }));
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_DeleteBetFromFoldersreturn0()
        {
            Assert.AreEqual(0, Bets.DeleteBetFromFolders(connectionString, 1, 1, new List<string>() { "liiga" }));
        }

        [Test]
        public void test_DeleteBetFromFoldersreturnminus1()
        {
            Assert.AreEqual(-1, Bets.DeleteBetFromFolders(connectionString, 1, 1, null));
            Assert.AreEqual(-1, Bets.DeleteBetFromFolders(connectionString, 1, 1, new List<string>()));
        }
    }
}
