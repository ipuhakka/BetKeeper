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
    }
}
