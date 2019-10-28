
using System.Collections.Generic;
using Betkeeper.Data;
using Betkeeper.Repositories;
using Betkeeper.Exceptions;
using NUnit.Framework;
using Moq;

namespace Test.Betkeeper.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        //UserRepository _UserRepository;

        //[OneTimeSetUp]
        //public void OneTimeSetup()
        //{
        //    _UserRepository = new UserRepository(new SQLDatabase());
        //    Tools.CreateTestDatabase();

        //    var setUpCommand = 
        //        "INSERT OR REPLACE INTO users (username, password, user_id) " +
        //            "VALUES ('username1', 'password1', 1);" +
        //        "INSERT INTO users (username, password, user_id) " +
        //            "VALUES ('username2', 'password2', 2);";
                
        //    Tools.ExecuteNonQuery(setUpCommand);
        //}

        //[OneTimeTearDown]
        //public void OneTimeTearDown()
        //{
        //    Tools.DeleteTestDatabase();
        //}

        [Test]
        public void AddUser_UsernameInUse_ThrowsUsernameInUseException()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(true);

            var userRepository = new UserRepository(mock.Object);

            Assert.Throws<UsernameInUseException>(() =>
                userRepository.AddUser("username", "password"));
        }

        [Test]
        public void AddUser_UniqueUsername_CorrectQueryFormed()
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
                    It.IsAny<bool>()))
                .Returns(1);

            var userRepository = new UserRepository(mock.Object);

            userRepository.AddUser("username", "password");

            mock.Verify(database =>
                database.ExecuteCommand("INSERT INTO users(username, password) " +
                "VALUES(@username, @password)",
                new Dictionary<string, object>
                    {
                        {"username", "username" },
                        {"password", "password" }
                    },
                    false), 
                Times.Once);
        }

        //[Test]
        //public void UserIdExists_DoesNotExist_ReturnsFalse()
        //{
        //    Assert.IsFalse(_UserRepository.UserIdExists(999));
        //}

        //[Test]
        //public void UserIdExists_ReturnsTrue()
        //{
        //    Assert.IsTrue(_UserRepository.UserIdExists(1));
        //}

        //[Test]
        //public void UserNameInUse_InUse_ReturnsTrue()
        //{
        //    Assert.IsTrue(_UserRepository.UsernameInUse("username1"));
        //}

        //[Test]
        //public void UserNameInUse_NotInUse_ReturnsFalse()
        //{
        //    Assert.IsFalse(_UserRepository.UsernameInUse("UnexistingUserName"));
        //}

        //[Test]
        //public void Authenticate_PasswordMatchesUserId_ReturnsTrue()
        //{
        //    Assert.IsTrue(_UserRepository.Authenticate(1, "password1"));
        //}

        //[Test]
        //public void Authenticate_PasswordDoesNotMatchUserId_ReturnsFalse()
        //{
        //    Assert.IsFalse(_UserRepository.Authenticate(1, "password2"));
        //}

        //[Test]
        //public void Authenticate_UserIdDoesNotExist_ReturnsFalse()
        //{
        //    Assert.IsFalse(_UserRepository.Authenticate(-1, "password2"));
        //}

        //[Test]
        //public void AddUser_UsernameExists_ThrowsUsernameInUseException()
        //{
        //    Assert.Throws<UsernameInUseException>(() =>
        //        _UserRepository.AddUser("username1", "password"));
        //}

        //[Test]
        //public void AddUser_UsernameDoesNotExist_ReturnsUserId()
        //{
        //    var result = _UserRepository.AddUser("username3", "password3");

        //    Assert.AreEqual(1, result);
        //}

        //[Test]
        //public void GetUserId_UsernameDoesNotExitsThrowsNotFoundException()
        //{
        //    Assert.Throws<NotFoundException>(() =>
        //    {
        //        _UserRepository.GetUserId("unexistingUser");
        //    });
        //}

        //[Test]
        //public void GetUserId_UsernameExists_ReturnsMatchingUserId()
        //{
        //    var userId1 = _UserRepository.GetUserId("username1");
        //    var userId2 = _UserRepository.GetUserId("username2");

        //    Assert.AreEqual(1, userId1);
        //    Assert.AreEqual(2, userId2);
        //}
    }
}
