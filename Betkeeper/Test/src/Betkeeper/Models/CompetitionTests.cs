using System;
using System.Collections.Generic;
using Betkeeper.Models;
using Betkeeper.Data;
using Betkeeper.Exceptions;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class CompetitionTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new BetkeeperDataContext(Tools.GetTestOptionsBuilder()))
            {
                context.Database.EnsureDeleted();
            }
        }

        public void GetCompetitionsById_ReturnsCompetitionsWithIdInList()
        {
            var competitions = new List<Competition>
            {
                new Competition
                {
                    CompetitionId = 1
                },
                new Competition
                {
                    CompetitionId = 2
                },
                new Competition
                {
                    CompetitionId = 3
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var results = new TestCompetitionRepository()
                .GetCompetitionsById(new List<int> { 1, 3, 4 });

            Assert.AreEqual(2, results.Count);

            Assert.AreEqual(1, results[0].CompetitionId);
            Assert.AreEqual(3, results[1].CompetitionId);
        }

        [Test]
        public void AddCompetition_NameInUse_ThrowsNameInUseException()
        {
            var inDatabaseCompetitions = new List<Competition>
            {
                new Competition()
                {
                    Name = "InUseName",
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    JoinCode = "679"
                }
            };

            var testCompetitions = new List<Competition>
            {
                new Competition()
                {
                    Name = "InUseName",
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    JoinCode = "456"
                }
            };

            Tools.CreateTestData(competitions: inDatabaseCompetitions);

            var repository = new TestRepository();

            testCompetitions.ForEach(competition =>
                Assert.Throws<NameInUseException>(() =>
                    repository.AddCompetition(competition)));
        }

        [Test]
        public void AddCompetition_JoinCodeInUse_ThrowsArgumentException()
        {
            var inDatabaseCompetitions = new List<Competition>
            {
                new Competition()
                {
                    Name = "NotInUseName",
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    JoinCode = "123"
                }
            };

            var testCompetitions = new List<Competition>
            {
                new Competition()
                {
                    Name = "Name",
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    JoinCode = "123"
                }
            };

            Tools.CreateTestData(competitions: inDatabaseCompetitions);

            var repository = new TestRepository();

            testCompetitions.ForEach(competition =>
                Assert.Throws<ArgumentException>(() =>
                    repository.AddCompetition(competition)));
        }

        [Test]
        public void AddCompetition_InvalidArgument_ThrowsArgumentOutOfRangeException()
        {
            var testCompetitions = new List<Competition>
            {
                // Empty name
                new Competition()
                {
                    CompetitionId = 2,
                    JoinCode = "123",
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    Description = "testikuvaus",
                    Name = ""
                },
                // Empty join code
                new Competition()
                {
                    CompetitionId = 3,
                    JoinCode = "",
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    Description = "testikuvaus",
                    Name = "name"
                },
                // Null name
                new Competition()
                {
                    CompetitionId = 4,
                    JoinCode = "123",
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    Description = "testikuvaus",
                    Name = null
                },
                // Null join code
                new Competition()
                {
                    CompetitionId = 5,
                    JoinCode = null,
                    StartTime = DateTime.UtcNow.AddDays(-1),
                    Description = "testikuvaus",
                    Name = "sdf"
                },
            };

            Tools.CreateTestData(competitions: testCompetitions);

            var repository = new TestRepository();

            testCompetitions.ForEach(competition =>
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    repository.AddCompetition(competition)));
        }

        [Test]
        public void AddCompetitions_ValidValues_Runs()
        {
            var competition = new Competition()
            {
                CompetitionId = 1,
                JoinCode = "123",
                Name = "Name",
                StartTime = DateTime.UtcNow.AddDays(-1)
            };

            var repository = new TestRepository();

            Assert.DoesNotThrow(() => repository.AddCompetition(competition));
        }

        [Test]
        public void DeleteCompetition_CompetitionNotFound_ThrowsInvalidOperationException()
        {
            var competitions = new List<Competition>
            {
                new Competition()
                {
                    CompetitionId = 999,
                    JoinCode = "test",
                    Name = "Name",
                    StartTime = DateTime.UtcNow.AddDays(-1)
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var repository = new TestRepository();

            Assert.Throws<InvalidOperationException>(() =>
                repository.DeleteCompetition(competitionId: 5));
        }

        [Test]
        public void DeleteCompetition_CompetitionExists_Runs()
        {
            var competitions = new List<Competition>
            {
                new Competition()
                {
                    CompetitionId = 3,
                    JoinCode = "test",
                    Name = "Name",
                    StartTime = DateTime.UtcNow.AddDays(-1)
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var repository = new TestRepository();

            Assert.DoesNotThrow(() =>
                repository.DeleteCompetition(competitionId: 3));
        }

        [Test]
        public void GetCompetition_CompetitionNotFound_ReturnsNull()
        {
            var competitions = new List<Competition>
            {
                new Competition{ CompetitionId = 1 },
                new Competition{ CompetitionId = 2 },
            };

            Tools.CreateTestData(competitions: competitions);

            Assert.IsNull(new TestRepository().GetCompetition(3));
        }

        [Test]
        public void GetCompetition_CompetitionFound_CompetitionReturned()
        {
            var competitions = new List<Competition>
            {
                new Competition{ CompetitionId = 1, Description = "Description 1" },
                new Competition{ CompetitionId = 2, Description = "Description 2" },
            };

            Tools.CreateTestData(competitions: competitions);

            var competition = new TestRepository().GetCompetition(1);

            Assert.AreEqual("Description 1", competition.Description);
        }

        private class TestRepository: CompetitionRepository
        {
            public TestRepository()
            {
                OptionsBuilder = Tools.GetTestOptionsBuilder();
            }
        }
    }
}
