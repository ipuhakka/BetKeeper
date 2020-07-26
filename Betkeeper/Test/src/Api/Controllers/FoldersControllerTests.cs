using Api.Classes;
using Api.Controllers;
using Betkeeper.Classes;
using Betkeeper.Repositories;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using TestTools;

namespace Api.Test.Controllers
{

    [TestFixture]
    public class FoldersControllerTests
    {

        [Test]
        public void Get_ValidTokenBetIdNull_ReturnsUsersFolders()
        {
            var mock = new Mock<IFolderRepository>();

            mock.Setup(folderRepository =>
                folderRepository.GetUsersFolders(It.IsAny<int>(), null)).Returns(
                new List<string>
                {
                    "folder1",
                    "folder2"
                });

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _FolderRepository = mock.Object
            };

            var response = controller.Get();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var folders = Http.GetHttpContent(response);

            Assert.AreEqual(2, folders.Count);
        }

        [Test]
        public void Get_ValidTokenBetId1_ReturnsUsersFolders()
        {
            var mock = new Mock<IFolderRepository>();

            mock.Setup(folderRepository =>
                folderRepository.GetUsersFolders(It.IsAny<int>(), null)).Returns(
                new List<string>
                {
                    "folder1",
                    "folder2"
                });

            mock.Setup(folderRepository =>
                folderRepository.GetUsersFolders(It.IsAny<int>(), 1)).Returns(
                new List<string>
                {
                    "folder1"
                });

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _FolderRepository = mock.Object
            };

            var response = controller.Get(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var folders = Http.GetHttpContent(response);

            Assert.AreEqual(1, folders.Count);
        }

        [Test]
        public void Get_InvalidToken_ReturnsUnauthorized()
        {
            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "unusedToken" }
                    })
            };

            var response = controller.Get();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Post_FolderMissing_ReturnsBadRequest()
        {
            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext()
            };

            var response = controller.Post(null);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Post_FolderLengthOver50_ReturnsBadRequest()
        {
            var tooLongFolderName = "This folder name has over 50 characters. " +
                "Column in SQLtable accepts up to 50 characters.";

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext()
            };

            var response = controller.Post(tooLongFolderName);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public void Post_InvalidToken_ReturnsUnauthorized()
        {
            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "unusedToken" }
                    })
            };

            var response = controller.Post("folderToAdd");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Post_UserHasFolder_ReturnsConflict()
        {
            var mock = new Mock<IFolderRepository>();

            mock.Setup(folderRepository =>
                folderRepository.UserHasFolder(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _FolderRepository = mock.Object
            };

            var response = controller.Post("folderToAdd");

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Test]
        public void Post_UserDoesNotHaveFolder_ReturnsCreated()
        {
            var mock = new Mock<IFolderRepository>();

            mock.Setup(folderRepository =>
                folderRepository.UserHasFolder(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            mock.Setup(folderRepository =>
                folderRepository.AddNewFolder(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(1);

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _FolderRepository = mock.Object
            };

            var response = controller.Post("folderToAdd");

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public void Delete_InvalidToken_ReturnsUnauthorized()
        {
            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", "unusedToken" }
                    })
            };

            var response = controller.Delete("folderToDelete");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public void Delete_UserDoesNotHaveFolder_ReturnsNotFound()
        {
            var mock = new Mock<IFolderRepository>();

            mock.Setup(folderRepository =>
                folderRepository.UserHasFolder(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _FolderRepository = mock.Object
            };

            var response = controller.Delete("folderToDelete");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void Delete_UserHasFolder_ReturnsNoContent()
        {
            var mock = new Mock<IFolderRepository>();

            mock.Setup(folderRepository =>
                folderRepository.UserHasFolder(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    }),
                _FolderRepository = mock.Object
            };

            var response = controller.Delete("folderToDelete");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
