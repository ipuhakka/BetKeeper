using System.Collections.Generic;
using Betkeeper.Data;
using Betkeeper.Repositories;
using Betkeeper.Exceptions;
using NUnit.Framework;
using Moq;

namespace Betkeeper.Test.Repositories
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
    }
}
