using Betkeeper.Models;
using Betkeeper.Exceptions;
using NUnit.Framework;

namespace Test.Betkeeper.Models
{
    [TestFixture]
    public class UserModelTests
    {
        UserModel _UserModel;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _UserModel = new UserModel();
            Tools.CreateTestDatabase();

            var setUpCommand = 
                "INSERT OR REPLACE INTO users (username, password, user_id) " +
                    "VALUES ('username1', 'password1', 1);" +
                "INSERT INTO users (username, password, user_id) " +
                    "VALUES ('username2', 'password2', 2);";
                
            Tools.ExecuteNonQuery(setUpCommand);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Tools.DeleteTestDatabase();
        }

        [Test]
        public void UserIdExists_DoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(_UserModel.UserIdExists(999));
        }

        [Test]
        public void UserIdExists_ReturnsTrue()
        {
            Assert.IsTrue(_UserModel.UserIdExists(1));
        }

        [Test]
        public void UserNameInUse_InUse_ReturnsTrue()
        {
            Assert.IsTrue(_UserModel.UsernameInUse("username1"));
        }

        [Test]
        public void UserNameInUse_NotInUse_ReturnsFalse()
        {
            Assert.IsFalse(_UserModel.UsernameInUse("UnexistingUserName"));
        }

        [Test]
        public void Authenticate_PasswordMatchesUserId_ReturnsTrue()
        {
            Assert.IsTrue(_UserModel.Authenticate(1, "password1"));
        }

        [Test]
        public void Authenticate_PasswordDoesNotMatchUserId_ReturnsFalse()
        {
            Assert.IsFalse(_UserModel.Authenticate(1, "password2"));
        }

        [Test]
        public void Authenticate_UserIdDoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(_UserModel.Authenticate(-1, "password2"));
        }

        [Test]
        public void AddUser_UsernameExists_ThrowsUsernameInUseException()
        {
            Assert.Throws<UsernameInUseException>(() =>
                _UserModel.AddUser("username1", "password"));
        }

        [Test]
        public void AddUser_UsernameDoesNotExist_ReturnsUserId()
        {
            var result = _UserModel.AddUser("username3", "password3");

            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetUserId_UsernameDoesNotExitsThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
            {
                _UserModel.GetUserId("unexistingUser");
            });
        }

        [Test]
        public void GetUserId_UsernameExists_ReturnsMatchingUserId()
        {
            var userId1 = _UserModel.GetUserId("username1");
            var userId2 = _UserModel.GetUserId("username2");

            Assert.AreEqual(1, userId1);
            Assert.AreEqual(2, userId2);
        }
    }
}
