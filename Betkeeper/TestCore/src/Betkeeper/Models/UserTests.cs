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
        private UserRepository _userRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
            _userRepository = new UserRepository();
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

            var usernames = _userRepository.GetUsernamesById(new List<int> { 1, 3 });

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

            Assert.IsNull(_userRepository.GetUserId("username"));
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

            Assert.AreEqual(2, _userRepository.GetUserId("username"));
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

            Assert.IsFalse(_userRepository.Authenticate(1, "secret2"));
        }

        [Test]
        public void Authenticate_IdAndPasswordMatch_ReturnsTrue()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Password = Security.Encrypt("secret")
                },
                new User
                {
                    UserId = 2,
                    Password = Security.Encrypt("secret2")
                }
            };

            Tools.CreateTestData(users: users);

            Assert.IsTrue(_userRepository.Authenticate(2, "secret2"));
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

            Assert.IsTrue(_userRepository.UsernameInUse("test"));
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

            Assert.IsFalse(_userRepository.UsernameInUse("test2"));
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
                _userRepository.AddUser("test", "secret"));
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

            _userRepository.AddUser("test2", "secret");

            Assert.AreEqual(2, _context.User.Count());
            Assert.AreEqual(1, _context.User.Count(user => user.Username == "test2"));
        }
    }
}
