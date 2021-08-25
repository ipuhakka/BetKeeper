using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Exceptions;
using Betkeeper.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TestTools;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class UserTests
    {
        private BetkeeperDataContext _context;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
            Tools.InitTestSecretKey();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _context.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _context.User.RemoveRange(_context.User);
            _context.SaveChanges();
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

            var userRepository = new UserRepository();
            var usernames = userRepository.GetUsernamesById(new List<int> { 1, 3 });

            Assert.AreEqual(2, usernames.Count);
            Assert.AreEqual(1, usernames.Count(user => user == "Username 1"));
            Assert.AreEqual(1, usernames.Count(user => user == "Username 3"));
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

            Assert.IsNull(new UserRepository().GetUserId("username"));
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

            Assert.AreEqual(2, new UserRepository().GetUserId("username"));
        }

        [Test]
        public void Authenticate_IdAndPasswordDoNotMatch_ReturnsFalse()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Password = Security.HashPlainText("secret", "testSalt"),
                    Salt = "testSalt"
                },
                new User
                {
                    UserId = 2,
                    Password = Security.HashPlainText("secret2", "testSalt2"),
                    Salt = "testSalt2"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.IsFalse(new UserRepository().Authenticate(1, "secret2"));
        }

        [Test]
        public void Authenticate_IdAndPasswordMatch_ReturnsTrue()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Password = Security.HashPlainText("secret", "salt"),
                    Salt = "salt"
                },
                new User
                {
                    UserId = 2,
                    Password = Security.HashPlainText("secret2", "salt2"),
                    Salt = "salt2"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.IsTrue(new UserRepository().Authenticate(2, "secret2"));
        }

        [Test]
        public void UsernameInUse_InUse_ReturnsTrue()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "test"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.IsTrue(new UserRepository().UsernameInUse("test"));
        }

        [Test]
        public void UsernameInUse_NotInUse_ReturnsFalse()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "test"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.IsFalse(new UserRepository().UsernameInUse("test2"));
        }

        [Test]
        public void AddUser_UsernameInUse_ThrowsException()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "test"
                }
            };

            Tools.CreateTestData(users: users);

            Assert.Throws<UsernameInUseException>(() =>
                new UserRepository().AddUser("test", "secret"));
        }

        [Test]
        public void AddUser_AddedSuccessfully()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Username = "test"
                }
            };

            Tools.CreateTestData(users: users);

            new UserRepository().AddUser("test2", "secret");

            Assert.AreEqual(2, _context.User.Count());
            Assert.AreEqual(1, _context.User.Count(user => user.Username == "test2"));
        }
    }
}
