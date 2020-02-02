using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Betkeeper;
using Betkeeper.Models;
using NUnit.Framework;
using Betkeeper;
using Betkeeper.Data;
using TestTools;

namespace Test.Betkeeper.Models
{
    public class ParticipatorTests
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
        public void AddParticipator_CompetitionDoesNotExist_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new TestParticipatorRepository(OptionsBuilder).AddParticipator(
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

            new TestParticipatorRepository(OptionsBuilder).AddParticipator(
                userId: 1, competitionId: 1, Enums.CompetitionRole.Host);

            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                Assert.AreEqual(1, context.Participators.ToList().Count);
            }
        }

        private class TestParticipatorRepository : ParticipatorRepository
        {
            public TestParticipatorRepository(DbContextOptionsBuilder optionsBuilder)
            {
                OptionsBuilder = optionsBuilder;
                CompetitionHandler = new TestCompetitionRepository(optionsBuilder);
            }

            private class TestCompetitionRepository : CompetitionRepository
            {
                public TestCompetitionRepository(DbContextOptionsBuilder optionsBuilder)
                {
                    OptionsBuilder = optionsBuilder;
                }
            }
        }
    }
}
