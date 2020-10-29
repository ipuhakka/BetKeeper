using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class FolderActionTests
    {
        private BetkeeperDataContext _context { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetp()
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
        public void UserHasFolder_True_ReturnsTrue()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            Assert.IsTrue(new FolderAction().UserHasFolder(1, "test"));
        }

        [Test]
        public void UserHasFolder_NoFolderWithNameExists_ReturnsFalse()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            Assert.IsFalse(new FolderAction().UserHasFolder(1, "test1"));
        }

        [Test]
        public void UserHasFolder_OnlyOtherUserHasFolder_ReturnsFalse()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            Assert.IsFalse(new FolderAction().UserHasFolder(2, "test"));
        }

        [Test]
        public void AddFolder_NameNullOrEmpty_ThrowsActionException()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    Owner = 1,
                    FolderName = ""
                },
                new Folder
                {
                    Owner = 1,
                    FolderName = null
                }
            };

            folders.ForEach(folder =>
            {
                Assert.Throws<ActionException>(() =>
                    new FolderAction().AddFolder(folder.Owner, folder.FolderName));
            });
        }

        [Test]
        public void AddFolder_UserHasFolder_ThrowsActionException()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    Owner = 1,
                    FolderName = "test"
                }
            };

            Tools.CreateTestData(folders: folders);

            Assert.Throws<ActionException>(() =>
                new FolderAction().AddFolder(1, "test"));
        }

        [Test]
        public void AddFolder_AddedSuccessfully()
        {
            new FolderAction().AddFolder(1, "test");

            Assert.AreEqual(1, _context.Folder.Count(folder => folder.FolderName == "test"));
        }

        [Test]
        public void DeleteFolder_UserDoesNotHaveFolder_ThrowsActionException()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 2
                },
                new Folder
                {
                    FolderName = "something",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            Assert.Throws<ActionException>(() => new FolderAction().DeleteFolder(1, "test"));
        }

        [Test]
        public void DeleteFolder_UserHasFolder_UsersFolderDeleted()
        {
            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 2
                },
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                }
            };

            Tools.CreateTestData(folders: folders);

            new FolderAction().DeleteFolder(1, "test");

            Assert.AreEqual(1, _context.Folder.Count());
        }

        [Test]
        public void DeleteBetFromFolders_UserMissingSomeFolder_ThrowsActionException()
        {
            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    FolderName = "test",
                    BetId = 1,
                    Owner = 1
                }
            };

            Tools.CreateTestData(betInBetFolders: betInBetFolders);

            try
            {
                new FolderAction().DeleteBetFromFolders(
                    1,
                    1,
                    new List<string> { "test", "test2" });

                Assert.Fail();
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionExceptionType.NotFound, e.ActionExceptionType);
            }
        }

        [Test]
        public void AddBetToFolders_BetAlreadyInSomeFolder_ThrowsActionException()
        {
            var betInBetFolders = new List<BetInBetFolder>
            {
                new BetInBetFolder
                {
                    BetId = 1,
                    FolderName = "test",
                    Owner = 1
                }
            };

            var folders = new List<Folder>
            {
                new Folder
                {
                    FolderName = "test",
                    Owner = 1
                },
                new Folder
                {
                    Owner = 1,
                    FolderName = "test2"
                }
            };

            Tools.CreateTestData(
                folders: folders,
                betInBetFolders: betInBetFolders);

            try
            {
                new FolderAction().AddBetToFolders(
                    1, 
                    1, 
                    new List<string>
                    {
                        "test",
                        "test2"
                    });
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionExceptionType.Conflict, e.ActionExceptionType);
            }
        }

        [Test]
        public void AddBetToFolders_UserDoesNotHaveSomeFolder_ThrowsActionException()
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
                    Owner = 2
                }
            };

            Tools.CreateTestData(folders: folders);

            try
            {
                new FolderAction().AddBetToFolders(
                    1,
                    1,
                    new List<string>
                    {
                        "test",
                        "test2"
                    });
            }
            catch (ActionException e)
            {
                Assert.AreEqual(ActionExceptionType.Conflict, e.ActionExceptionType);
            }
        }

        [Test]
        public void AddBetToFolders_AddsBetToFolders()
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

            Tools.CreateTestData(folders: folders);

            new FolderAction().AddBetToFolders(
                    1,
                    1,
                    new List<string>
                    {
                        "test",
                        "test2"
                    });

            var betInBetFolders = _context.BetInBetFolder.ToList();

            Assert.AreEqual(2, betInBetFolders.Count);
        }
    }
}
