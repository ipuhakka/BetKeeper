using Api.Classes;
using Api.Controllers;
using Betkeeper;
using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using TestTools;

namespace Api.Test.Controllers
{

    [TestFixture]
    public class FoldersControllerTests
    {
        BetkeeperDataContext _context { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _context = Tools.GetTestContext();
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Folder.RemoveRange(_context.Folder);
            _context.BetInBetFolder.RemoveRange(_context.BetInBetFolder);
            _context.SaveChanges();
        }

        [Test]
        public void Get_ValidTokenBetIdNull_ReturnsUsersFolders()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "folder1",
                    Owner = 1
                },
                new Folder
                {
                    FolderName = "folder2",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Get();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var returnedFolders = Http.GetHttpContent(response);

            Assert.AreEqual(2, returnedFolders.Count);
        }

        [Test]
        public void Get_ValidTokenBetId1_ReturnsUsersFolders()
        {
            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    FolderName = "test",
                    Owner = 1,
                    BetId = 1
                },
                new BetInBetFolder
                {
                    FolderName = "test2",
                    Owner = 1,
                    BetId = 2
                }
            };

            Tools.CreateTestData(betInBetFolders: betInBetFolders);

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
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
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "folderToAdd",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Post("folderToAdd");

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Test]
        public void Post_UserDoesNotHaveFolder_ReturnsCreated()
        {
            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
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
            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Delete("folderToDelete");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public void Delete_UserHasFolder_ReturnsNoContent()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "folderToDelete",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            var token = TokenLog.CreateToken(1);

            var controller = new FoldersController()
            {
                ControllerContext = Tools.MockHttpControllerContext(
                    headers: new Dictionary<string, string>
                    {
                        { "Authorization", token.TokenString }
                    })
            };

            var response = controller.Delete("folderToDelete");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
