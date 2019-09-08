using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Models;
using Betkeeper.Exceptions;
using NUnit.Framework;

namespace Test.Models
{
    [TestFixture]
    public class UserTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
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
        public void UserNameInUse_InUse_ReturnsTrue()
        {
            Assert.IsTrue(User.UsernameInUse("username1"));
        }

        [Test]
        public void UserNameInUse_NotInUse_ReturnsFalse()
        {
            Assert.IsFalse(User.UsernameInUse("UnexistingUserName"));
        }

        [Test]
        public void Authenticate_PasswordMatchesUserId_ReturnsTrue()
        {
            Assert.IsTrue(User.Authenticate(1, "password1"));
        }

        [Test]
        public void Authenticate_PasswordDoesNotMatchUserId_ReturnsFalse()
        {
            Assert.IsFalse(User.Authenticate(1, "password2"));
        }

        [Test]
        public void Authenticate_UserIdDoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(User.Authenticate(-1, "password2"));
        }

        [Test]
        public void AddUser_UsernameExists_ThrowsUsernameInUseException()
        {
            Assert.Throws<UsernameInUseException>(() =>
                User.AddUser("username1", "password"));
        }

        [Test]
        public void AddUser_UsernameDoesNotExist_ReturnsUserId()
        {
            var result = User.AddUser("username3", "password3");

            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetUserId_UsernameDoesNotExitsThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
            {
                User.GetUserId("unexistingUser");
            });
        }

        [Test]
        public void GetUserId_UsernameExists_ReturnsMatchingUserId()
        {
            var userId1 = User.GetUserId("username1");
            var userId2 = User.GetUserId("username2");

            Assert.AreEqual(1, userId1);
            Assert.AreEqual(2, userId2);
        }
    }
}
