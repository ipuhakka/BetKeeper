
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

        [Test]
        public void GetUserId_UsernameDoesNotExist_Throws_NotFoundException()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(false);

            var userRepository = new UserRepository(mock.Object);

            Assert.Throws<NotFoundException>(() =>
                userRepository.GetUserId("username"));
        }

        [Test]
        public void GetUserId_UsernameExists_CorrectQueryFormed()
        {
            var mock = new Mock<IDatabase>();

            mock.Setup(database =>
                database.ReadBoolean(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(true);

            mock.Setup(database =>
                database.ReadInt(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Returns(1);

            var userRepository = new UserRepository(mock.Object);

            userRepository.GetUserId("username");

            mock.Verify(database =>
                database.ReadInt("SELECT user_id FROM users " +
                    "WHERE username=@username",
                new Dictionary<string, object>
                    {
                        {"username", "username" }
                    }),
                Times.Once);
        }

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
