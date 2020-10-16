using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Models;
using Betkeeper.Actions;
using Betkeeper.Data;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class UserActionTests
    {
        private UserAction userAction;

        private BetkeeperDataContext _context;

        [SetUp]
        public void SetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
            userAction = new UserAction();
        }

        [TearDown]
        public void TearDown()
        {
            _context.User.RemoveRange(_context.User);
            _context.SaveChanges();
        }

        [Test]
        public void ChangePassword_ConfirmDoesNotMatch_ThrowsActionException()
        {
            try
            {
                userAction.ChangePassword(1, "test", "not the same");
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual("Passwords do not match", e.ErrorMessage);
            }
        }

        [Test]
        public void ChangePassword_PasswordSameAsPrevious_ThrowsActionException()
        {
            Tools.CreateTestData(users: new List<User>
            {
                new User
                {
                    UserId = 1,
                    Password = "test"
                }
            });

            try
            {
                userAction.ChangePassword(1, "test", "test");
                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual("Password cannot be same as previous one", e.ErrorMessage);
            }
        }

        [Test]
        public void ChangePassword_PasswordOk_PasswordChanged()
        {
            Tools.CreateTestData(users: new List<User>
            {
                new User
                {
                    UserId = 1,
                    Password = "test"
                }
            });

            userAction.ChangePassword(1, "test2", "test2");

            var user = new UserRepository().GetUsersById(new List<int> { 1 }).Single();
            Assert.AreEqual("test2", user.Password);
        }
    }
}
