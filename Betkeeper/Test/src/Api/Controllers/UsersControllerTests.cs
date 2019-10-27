using System.Collections.Generic;
using System.Net;
using Api.Controllers;
using Betkeeper.Repositories;
using NUnit.Framework;
using Moq;

namespace Test.Api.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        [Test]
        public void Post_UsernameMissing_ReturnsBadRequest()
        {
            var controller = new UsersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "password" }
                    })
            };

            var response = controller.Post();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public void Post_PasswordMissing_ReturnsBadRequest()
        {
            var controller = new UsersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        username = "username"
                    })
            };

            var response = controller.Post();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public void Post_UsernameInUse_ReturnsConflict()
        {
            var mock = new Mock<IUserRepository>();

            mock.Setup(userRepo => 
                userRepo.UsernameInUse(It.IsAny<string>()))
                .Returns(true);

            var controller = new UsersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        username = "username"
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "password" }
                    }),
                _UserRepository = mock.Object
            };

            var response = controller.Post();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Conflict);
        }

        [Test]
        public void Post_RequestOK_ReturnsCreated()
        {
            var mock = new Mock<IUserRepository>();

            mock.Setup(userRepo =>
                userRepo.UsernameInUse(It.IsAny<string>()))
                .Returns(false);

            mock.Setup(userRepo =>
                userRepo.AddUser(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(1);

            var controller = new UsersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    dataContent: new
                    {
                        username = "username"
                    },
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "password" }
                    }),
                _UserRepository = mock.Object
            };

            var response = controller.Post();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

            mock.Verify(userRepo =>
                userRepo.AddUser("username", "password"), Times.Once);
        }
    }
}
