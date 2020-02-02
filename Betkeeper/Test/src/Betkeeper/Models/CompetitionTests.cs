using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Betkeeper.Models;
using Betkeeper.Data;
using Betkeeper;
using NUnit.Framework;
using TestTools;

namespace Test.Betkeeper.Models
{
    [TestFixture]
    public class CompetitionTests
    {
        DbContextOptionsBuilder OptionsBuilder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";

            OptionsBuilder = new DbContextOptionsBuilder()
                .UseInMemoryDatabase("testDatabase");
        }

        [TearDown]
        public void TearDown()
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                context.Database.EnsureDeleted();
            }
        }

        [Test]
        public void AddCompetition_InvalidArgument_ThrowsArgumentOutOfRangeException()
        {
            var testCompetitions = new List<Competition>
            {
                // State out of range
                new Competition()
                {
                    CompetitionId = 1,
                    JoinCode = "123",
                    StartTime = new DateTime(),
                    State = -1,
                    Description = "testikuvaus",
                    Name = "nimi"
                },
                // Empty name
                new Competition()
                {
                    CompetitionId = 2,
                    JoinCode = "123",
                    StartTime = new DateTime(),
                    State = 1,
                    Description = "testikuvaus",
                    Name = ""
                },
                // Empty join code
                new Competition()
                {
                    CompetitionId = 3,
                    JoinCode = "",
                    StartTime = new DateTime(),
                    State = 1,
                    Description = "testikuvaus",
                    Name = "name"
                },
                // Null name
                new Competition()
                {
                    CompetitionId = 4,
                    JoinCode = "123",
                    StartTime = new DateTime(),
                    State = 1,
                    Description = "testikuvaus",
                    Name = null
                },
                // Null join code
                new Competition()
                {
                    CompetitionId = 5,
                    JoinCode = null,
                    StartTime = new DateTime(),
                    State = 1,
                    Description = "testikuvaus",
                    Name = "sdf"
                },
            };

            Tools.CreateTestData(competitions: testCompetitions);

            var repository = new TestRepository(OptionsBuilder);

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
                State = 1
            };

            var repository = new TestRepository(OptionsBuilder);

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
                    State = 2
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var repository = new TestRepository(OptionsBuilder);

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
                    State = 2
                }
            };

            Tools.CreateTestData(competitions: competitions);

            var repository = new TestRepository(OptionsBuilder);

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

            Assert.IsNull(new TestRepository(OptionsBuilder).GetCompetition(3));
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

            var competition = new TestRepository(OptionsBuilder).GetCompetition(1);

            Assert.AreEqual("Description 1", competition.Description);
        }

        private class TestRepository: CompetitionRepository
        {
            public TestRepository(DbContextOptionsBuilder optionsBuilder)
            {
                OptionsBuilder = optionsBuilder;
            }
        }
    }
}
