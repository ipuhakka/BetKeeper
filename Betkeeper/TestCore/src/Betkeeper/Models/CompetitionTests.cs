using Betkeeper.Data;
using Betkeeper.Exceptions;
using Betkeeper.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestTools;

namespace Betkeeper.Test.Models
{
    [TestFixture]
    public class CompetitionTests
    {
        private BetkeeperDataContext _context;

        private CompetitionRepository _competitionRepository;

        [SetUp]
        public void SetUp()
        {
            _context = Tools.GetTestContext();
            _competitionRepository = new CompetitionRepository();
            Settings.InitializeOptionsBuilderService(Tools.GetTestOptionsBuilder());
        }

        [TearDown]
        public void TearDown()
        {
            _context.Competition.RemoveRange(_context.Competition);
            _context.SaveChanges();
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

            var results = _competitionRepository
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

            testCompetitions.ForEach(competition =>
                Assert.Throws<NameInUseException>(() =>
                    _competitionRepository.AddCompetition(competition)));
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

            testCompetitions.ForEach(competition =>
                Assert.Throws<ArgumentException>(() =>
                    _competitionRepository.AddCompetition(competition)));
        }

        [Test]
        public void AddCompetition_InvalidArgument_ThrowsArgumentException()
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

            testCompetitions.ForEach(competition =>
                Assert.Throws<ArgumentException>(() =>
                    _competitionRepository.AddCompetition(competition)));
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

            Assert.DoesNotThrow(() => _competitionRepository.AddCompetition(competition));
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

            Assert.Throws<InvalidOperationException>(() =>
                _competitionRepository.DeleteCompetition(competitionId: 5));
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

            Assert.DoesNotThrow(() =>
                _competitionRepository.DeleteCompetition(competitionId: 3));
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

            Assert.IsNull(_competitionRepository.GetCompetition(3));
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

            var competition = _competitionRepository.GetCompetition(1);

            Assert.AreEqual("Description 1", competition.Description);
        }
    }
}
