using System;
using System.Collections.Generic;
using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Models;
using TestTools;
using NUnit.Framework;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class CompetitionActionsTest
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
        public void CreateCompetition_NameInUse_ThrowsArgumentException()
        {
            var inDatabaseCompetitions = new List<Competition>
            {
                new Competition()
                {
                    CompetitionId = 1,
                    Name = "InUseName",
                    JoinCode = "213"
                }
            };

            Tools.CreateTestData(competitions: inDatabaseCompetitions);

            var testAction = new TestAction();

           Assert.Throws<ArgumentException>(() =>
            testAction.CreateCompetition(1, "InUseName", "Description", new DateTime(2000, 1, 1)));
        }

        [Test]
        public void CreateCompetition_CreatesParticipatorAndCompetition()
        {
            var testAction = new TestAction();

            testAction.CreateCompetition(1, "TestName", "Description to remember", new DateTime(2000, 1, 1));

            var competitions = new TestCompetitionRepository().GetCompetitions();

            Assert.AreEqual(1, competitions.Count);

            Assert.AreEqual("Description to remember", competitions[0].Description);

            Assert.AreEqual((int)Enums.CompetitionState.Open, competitions[0].State);
        }

        private class TestAction : CompetitionAction
        {
            public TestAction()
            {
                CompetitionRepository = new TestCompetitionRepository();
                ParticipatorRepository = new TestParticipatorRepository();
            }
        }

        private class TestCompetitionRepository : CompetitionRepository
        {
            public TestCompetitionRepository()
            {
                OptionsBuilder = Tools.GetTestOptionsBuilder();
            }
        }

        private class TestParticipatorRepository : ParticipatorRepository
        {
            public TestParticipatorRepository()
            {
                OptionsBuilder = Tools.GetTestOptionsBuilder();
                CompetitionHandler = new TestCompetitionRepository();
            }
        }
    }
}
