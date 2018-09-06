using System.IO;
using BetKeeper.DB;
using BetKeeper.DB.Tables;
using NUnit.Framework;

namespace Test.BetKeeper.DB.Tables
{
    [TestFixture]
    [Category("Database")]
    class Users_tests
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
        public void test_AddUser_already_exists()
        {
            Assert.AreEqual(-1, Users.AddUser(connectionString, "jannu27", "somepassword"));
        }

        [Test]
        public void test_AddUser()
        {
            Assert.AreEqual(1, Users.AddUser(connectionString, "newuser", "somepassword"));
            db.ClearTables();
            db.FillTables("db_testdata_dump.sql");
        }

        [Test]
        public void test_PasswordIsCorrect_return_true()
        {
            Assert.IsTrue(Users.PasswordIsCorrect(connectionString, 1, "salasana"));
        }

        [Test]
        public void test_PasswordIsCorrect_return_false()
        {
            Assert.IsFalse(Users.PasswordIsCorrect(connectionString, 1, "wrongpassword"));
        }

        [Test]
        public void test_PasswordIsCorrect_return_false_unexisting_user()
        {
            Assert.IsFalse(Users.PasswordIsCorrect(connectionString, 999, "wrongpassword"));
        }

        [Test]
        public void test_GetUserId_return_minus1()
        {
            Assert.AreEqual(-1, Users.GetUserId(connectionString, "unexistinguser"));
        }

        [Test]
        public void test_GetUserId_return_3()
        {
            Assert.AreEqual(3, Users.GetUserId(connectionString, "käyttäjä2"));
        }
    }
}
