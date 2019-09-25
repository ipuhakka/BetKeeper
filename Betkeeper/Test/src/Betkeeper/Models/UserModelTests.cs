using Betkeeper.Models;
using Betkeeper.Exceptions;
using NUnit.Framework;

namespace Test.Betkeeper.Models
{
    [TestFixture]
    public class UserModelTests
    {
        UserModel userModel;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            userModel = new UserModel();
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
            Assert.IsFalse(userModel.UserIdExists(999));
        }

        [Test]
        public void UserIdExists_ReturnsTrue()
        {
            Assert.IsTrue(userModel.UserIdExists(1));
        }

        [Test]
        public void UserNameInUse_InUse_ReturnsTrue()
        {
            Assert.IsTrue(userModel.UsernameInUse("username1"));
        }

        [Test]
        public void UserNameInUse_NotInUse_ReturnsFalse()
        {
            Assert.IsFalse(userModel.UsernameInUse("UnexistingUserName"));
        }

        [Test]
        public void Authenticate_PasswordMatchesUserId_ReturnsTrue()
        {
            Assert.IsTrue(userModel.Authenticate(1, "password1"));
        }

        [Test]
        public void Authenticate_PasswordDoesNotMatchUserId_ReturnsFalse()
        {
            Assert.IsFalse(userModel.Authenticate(1, "password2"));
        }

        [Test]
        public void Authenticate_UserIdDoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(userModel.Authenticate(-1, "password2"));
        }

        [Test]
        public void AddUser_UsernameExists_ThrowsUsernameInUseException()
        {
            Assert.Throws<UsernameInUseException>(() =>
                userModel.AddUser("username1", "password"));
        }

        [Test]
        public void AddUser_UsernameDoesNotExist_ReturnsUserId()
        {
            var result = userModel.AddUser("username3", "password3");

            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetUserId_UsernameDoesNotExitsThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
            {
                userModel.GetUserId("unexistingUser");
            });
        }

        [Test]
        public void GetUserId_UsernameExists_ReturnsMatchingUserId()
        {
            var userId1 = userModel.GetUserId("username1");
            var userId2 = userModel.GetUserId("username2");

            Assert.AreEqual(1, userId1);
            Assert.AreEqual(2, userId2);
        }
    }
}
