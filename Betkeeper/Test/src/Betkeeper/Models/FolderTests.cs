using System;
using System.Collections.Generic;
using NUnit.Framework;
using Betkeeper.Models;
using Betkeeper.Data;
using TestTools;
using System.Linq;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class FolderTests
    {
        private BetkeeperDataContext _context;

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
        public void GetFolders_ReturnsUsersFolders()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test1",
                    Owner = 1
                },
                new Folder
                {
                    FolderName = "test2",
                    Owner = 2
                },
                new Folder
                {
                    FolderName = "test3",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            var usersFolders = new FolderRepository().GetFolders(1);

            Assert.AreEqual(2, usersFolders.Count);
            Assert.AreEqual("test1", usersFolders[0].FolderName);
            Assert.AreEqual("test3", usersFolders[1].FolderName);
        }

        [Test]
        public void GetFolders_WithFolder_ReturnsUsersFolder()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test1",
                    Owner = 1
                },
                new Folder
                {
                    FolderName = "test2",
                    Owner = 2
                },
                new Folder
                {
                    FolderName = "test3",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            var usersFolders = new FolderRepository().GetFolders(1, "test1");

            Assert.AreEqual(1, usersFolders.Count);
            Assert.AreEqual("test1", usersFolders[0].FolderName);
        }

        [Test]
        public void GetUsersFolders_WithBetId_ReturnsFoldersWhichHaveBet()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                },
                new Folder
                {
                    FolderName = "test2",
                    Owner = 1
                }
            };

            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(
                folders: folders,
                betInBetFolders: betInBetFolders);

            var folderNames = new FolderRepository().GetUsersFolders(userId: 1, betId: 1);

            Assert.AreEqual(1, folderNames.Count);
            Assert.AreEqual("test", folderNames[0]);
        }

        [Test]
        public void GetUsersFolder_NoBetId_ReturnsUsersAllFolders()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                },
                new Folder
                {
                    FolderName = "test2",
                    Owner = 1
                }
            };

            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(
                folders: folders,
                betInBetFolders: betInBetFolders);

            var folderNames = new FolderRepository().GetUsersFolders(userId: 1);

            Assert.AreEqual(2, folderNames.Count);
        }

        [Test]
        public void FolderHasBet_Yes_ReturnsTrue()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                }
            };

            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(
                folders: folders,
                betInBetFolders: betInBetFolders);

            Assert.IsTrue(
                new FolderRepository()
                    .FolderHasBet(1, "test", 1));
        }

        [Test]
        public void FolderHasBet_No_ReturnsFalse()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                }
            };

            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(
                folders: folders,
                betInBetFolders: betInBetFolders);

            Assert.IsFalse(
                new FolderRepository()
                    .FolderHasBet(1, "test", 2));
        }

        [Test]
        public void DeleteBetFromFolders_RemovesBetFromUsersFolders()
        {
            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "test",
                    Owner = 1
                },
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "test2",
                    Owner = 1
                },
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "test3",
                    Owner = 1
                },
                new BetInBetFolder
                {
                    BetId = 2,
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(betInBetFolders: betInBetFolders);

            new FolderRepository().DeleteBetFromFolders(
                1,
                1,
                new List<string>
                {
                    "test",
                    "test2"
                });

            var results = _context.BetInBetFolder.ToList();

            Assert.AreEqual(2, results.Count);
        }
    }
}
