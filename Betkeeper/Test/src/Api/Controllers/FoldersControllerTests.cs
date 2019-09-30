using System.Net;
using System.Collections.Generic;
using Betkeeper.Models;
using Betkeeper.Data;
using Api.Classes;
using Api.Controllers;
using NUnit.Framework;
using Moq;

namespace Test.Api.Controllers
{

    [TestFixture]
    public class FoldersControllerTests
    {

        [Test]
        public void Get_ValidTokenBetIdNull_ReturnsUsersFolders()
        {
            var mock = new Mock<IFolderModel>();

            mock.Setup(folderModel =>
                folderModel.GetUsersFolders(It.IsAny<int>(), null)).Returns(
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
                _FolderModel = mock.Object
            };

            var response = controller.Get();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var folders = Http.GetHttpContent(response);

            Assert.AreEqual(2, folders.Count);
        }

        [Test]
        public void Get_ValidTokenBetId1_ReturnsUsersFolders()
        {
            var mock = new Mock<IFolderModel>();

            mock.Setup(folderModel =>
                folderModel.GetUsersFolders(It.IsAny<int>(), null)).Returns(
                new List<string>
                {
                    "folder1",
                    "folder2"
                });

            mock.Setup(folderModel =>
                folderModel.GetUsersFolders(It.IsAny<int>(), 1)).Returns(
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
                _FolderModel = mock.Object
            };

            var response = controller.Get(1);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var folders = Http.GetHttpContent(response);

            Assert.AreEqual(1, folders.Count);
        }

        [Test]
        public void Get_InvalidTokenReturnsUnauthorized()
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
    }
}
