using Betkeeper.Data;
using Betkeeper.Models;
using NUnit.Framework;
using System.Collections.Generic;
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
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";
            _context = new BetkeeperDataContext(Tools.GetTestOptionsBuilder());
            _userRepository = new UserRepository(_context);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _context.Dispose();
            _userRepository.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _context.User.RemoveRange(_context.User);
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

            Tools.CreateTestData(_context, users: users);

            var usernames = _userRepository.GetUsernamesById(new List<int> { 1, 3 });

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

            Tools.CreateTestData(_context, users: users);

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

            Tools.CreateTestData(_context, users: users);

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

            Tools.CreateTestData(_context, users: users);

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
                    Password = "secret"
                },
                new User
                {
                    UserId = 2,
                    Password = "secret2"
                }
            };

            Tools.CreateTestData(_context, users: users);

            Assert.IsTrue(_userRepository.Authenticate(2, "secret2"));
        }
    }
}
