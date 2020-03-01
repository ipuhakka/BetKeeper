using System.Collections.Generic;
using Betkeeper.Data;
using Betkeeper.Models;
using TestTools;
using NUnit.Framework;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class UserTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new BetkeeperDataContext(Tools.GetTestOptionsBuilder()))
            {
                context.Database.EnsureDeleted();
            }
        }

        [Test]
        public void GetUserNamesById_ReturnsCorrectUsernames()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "Username 1"
                },
                new User
                {
                    UserId = 2,
                    Username = "Username 2"
                },
                new User
                {
                    UserId = 3,
                    Username = "Username 3"
                }
            };

            Tools.CreateTestData(users: users);

            var usernames = new TestUserRepository().GetUsernamesById(new List<int> { 1, 3 });

            Assert.AreEqual(2, usernames.Count);
            Assert.AreEqual("Username 1", usernames[0]);
            Assert.AreEqual("Username 3", usernames[1]);
        }

        [Test]
        public void GetUserId_UsernameDoesNotExist_ReturnsNull()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "Not searched username"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.IsNull(new TestUserRepository().GetUserId("username"));
        }

        [Test]
        public void GetUserId_UsernameExists_ReturnsUserId()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "Not searched username"
                },
                new User
                {
                    UserId = 2,
                    Username = "username"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.AreEqual(2, new TestUserRepository().GetUserId("username"));
        }

        [Test]
        public void Authenticate_IdAndPasswordDoNotMatch_ReturnsFalse()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Password = "secret"
                },
                new User
                {
                    UserId = 2,
                    Password = "secret2"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.IsFalse(new TestUserRepository().Authenticate(1, "secret2"));
        }

        [Test]
        public void Authenticate_IdAndPasswordMatch_ReturnsTrue()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Password = "secret"
                },
                new User
                {
                    UserId = 2,
                    Password = "secret2"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.IsTrue(new TestUserRepository().Authenticate(2, "secret2"));
        }
    }
}
