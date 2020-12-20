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
    }
}
