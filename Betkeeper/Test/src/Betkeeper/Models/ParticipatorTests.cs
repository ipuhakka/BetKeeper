using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Models;
using NUnit.Framework;
using Betkeeper.Data;
using TestTools;

namespace Betkeeper.Test.Models
{
    public class ParticipatorTests
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

        [Test]
        public void GetParticipators_FiltersCorrectly()
        {
            var competitions = new List<Competition>
            {
                new Competition()
                {
                    JoinCode = "joincode",
                    Name = "Name",
                    CompetitionId = 1
                },
                new Competition()
                {
                    JoinCode = "joincode2",
                    Name = "Name2",
                    CompetitionId = 2
                }
            };

            var participators = new List<Participator>
            {
                new Participator
                {
                    Competition = 1,
                    UserId = 1
                },
                new Participator
                {
                    Competition = 2,
                    UserId = 1
                },
                new Participator
                {
                    Competition = 2,
                    UserId = 2
                },
                new Participator
                {
                    Competition = 1,
                    UserId = 2
                }
            };

            Tools.CreateTestData(competitions: competitions, participators: participators);

            var testRepo = new TestParticipatorRepository();

            Assert.AreEqual(4, testRepo.GetParticipators().Count);
            Assert.AreEqual(2, testRepo.GetParticipators(competitionId: 2).Count);
            Assert.AreEqual(2, testRepo.GetParticipators(userId: 2).Count);
            Assert.AreEqual(1, testRepo.GetParticipators(userId: 1, competitionId: 1).Count);
        }

        [Test]
        public void AddParticipator_CompetitionDoesNotExist_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new TestParticipatorRepository().AddParticipator(
                    userId: 1, competitionId: 1, Enums.CompetitionRole.Host));
        }

        [Test]
        public void AddParticipator_CompetitionExistst_ParticipatorAdded()
        {
            var competitions = new List<Competition>
            {
                new Competition()
                {
                    JoinCode = "joincode",
                    Name = "Name",
                    CompetitionId = 1
                }
            };

            Tools.CreateTestData(competitions: competitions);

            new TestParticipatorRepository().AddParticipator(
                userId: 1, competitionId: 1, Enums.CompetitionRole.Host);

            using (var context = new BetkeeperDataContext(Tools.GetTestOptionsBuilder()))
            {
                Assert.AreEqual(1, context.Participator.ToList().Count);
            }
        }

        private class TestParticipatorRepository : ParticipatorRepository
        {
            public TestParticipatorRepository()
            {
                OptionsBuilder = Tools.GetTestOptionsBuilder();
                CompetitionHandler = new TestCompetitionRepository();
            }

            private class TestCompetitionRepository : CompetitionRepository
            {
                public TestCompetitionRepository()
                {
                    OptionsBuilder = Tools.GetTestOptionsBuilder();
                }
            }
        }
    }
}
