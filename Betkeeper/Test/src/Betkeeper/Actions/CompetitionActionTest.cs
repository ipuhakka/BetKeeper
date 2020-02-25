using System;
using System.Collections.Generic;
using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Models;
using TestTools;
using NUnit.Framework;
using Betkeeper.Exceptions;

namespace Betkeeper.Test.Actions
{
    [TestFixture]
    public class CompetitionActionTest
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
        public void GetUsersCompetitions_ReturnsUsersCompetitions()
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

            var participators = new List<Participator>
            {
                new Participator
                {
                    UserId = 1,
                    Competition = 1
                },
                new Participator
                {
                    UserId = 2,
                    Competition = 3
                },
                new Participator
                {
                    UserId = 1,
                    Competition = 2
                },
                new Participator
                {
                    UserId = 2,
                    Competition = 2
                }
            };

            Tools.CreateTestData(participators: participators, competitions: competitions);

            var resultCompetitions = new TestAction().GetUsersCompetitions(1);

            Assert.AreEqual(2, resultCompetitions.Count);
            Assert.AreEqual(1, resultCompetitions[0].CompetitionId);
            Assert.AreEqual(2, resultCompetitions[1].CompetitionId);
        }

        [Test]
        public void CreateCompetition_NameInUse_ThrowsNameInUseExceptionException()
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

           Assert.Throws<NameInUseException>(() =>
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
    }
}
