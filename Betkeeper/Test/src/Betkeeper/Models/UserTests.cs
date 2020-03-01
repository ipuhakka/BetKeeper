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
    }
}
