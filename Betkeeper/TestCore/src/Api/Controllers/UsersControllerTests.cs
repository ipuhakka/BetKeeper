using Api.Controllers;
using Betkeeper;
using Betkeeper.Data;
using Betkeeper.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TestTools;

namespace Api.Test.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        BetkeeperDataContext _context;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
            _context = Tools.GetTestContext();
            Tools.InitTestSecretKey();
        }

        [TearDown]
        public void TearDown()
        {
            _context.User.RemoveRange(_context.User);
            _context.SaveChanges();
        }

        [Test]
        public void Post_UsernameMissing_ReturnsBadRequest()
        {
            var controller = new UsersController()
            {
                ControllerContext = Tools.MockControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "password" }
                    })
            };

            var response = controller.Post() as BadRequestObjectResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Post_PasswordMissing_ReturnsBadRequest()
        {
            var controller = new UsersController()
            {
                ControllerContext = Tools.MockControllerContext(
                    dataContent: new
                    {
                        username = "username"
                    })
            };

            var response = controller.Post() as BadRequestObjectResult;

            Assert.AreEqual(400, response.StatusCode);
        }

        [Test]
        public void Post_UsernameInUse_ReturnsConflict()
        {
            var users = new List<User>
            {
                new User
                {
                    Username = "username"
                }
            };

            Tools.CreateTestData(users: users);

            var controller = new UsersController()
            {
                ControllerContext = Tools.MockControllerContext(
                    dataContent: new
                    {
                        username = "username"
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "password" }
                    })
            };

            var response = controller.Post() as ConflictObjectResult;

            Assert.AreEqual(409, response.StatusCode);
        }

        [Test]
        public void Post_RequestOK_ReturnsCreated()
        {
            var controller = new UsersController()
            {
                ControllerContext = Tools.MockControllerContext(
                    dataContent: new
                    {
                        username = "username"
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "password" }
                    })
            };

            var response = controller.Post() as OkResult;

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(1, _context.User.Count(user => user.Username == "username"));
        }
    }
}
