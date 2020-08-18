using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Betkeeper.Actions;
using Betkeeper.Data;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Pages.CompetitionPage;
using NUnit.Framework;
using TestTools;

namespace Betkeeper.Test.Pages.CompetitionPageTests
{
    [TestFixture]
    public class ManageBetsTests
    {
        private BetkeeperDataContext _context;
        private CompetitionAction _competitionAction;
        private TargetAction _targetAction;
        private CompetitionPage _competitionPage;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Set Connectionstring so base constructor runs
            Settings.ConnectionString = "TestDatabase";

            _context = Tools.GetTestContext();

            var competitionRepository = new CompetitionRepository(_context);
            var participatorRepository = new ParticipatorRepository(_context);
            var targetRepository = new TargetRepository(_context);

            Tools.CreateTestData(
                _context,
                competitions: new List<Competition>
                {
                    new Competition
                    {
                        CompetitionId = 1,
                        // Set competition as open
                        StartTime = DateTime.Today.AddDays(1)
                    }
                });

            _competitionAction = new CompetitionAction(competitionRepository, participatorRepository);
            _targetAction = new TargetAction(competitionRepository, participatorRepository, targetRepository);

            _competitionPage = new CompetitionPage(_competitionAction, _targetAction, new TargetBetAction(_context));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _competitionPage.Dispose();
        }

        /// <summary>
        /// Tests that new targets are created and existing one's updated.
        /// </summary>
        [Test]
        public void SaveBetTargets_ValidTargetsAreCreated()
        {
            var updateTarget = new Target
            {
                Bet = "Bet",
                Type = Enums.TargetType.OpenQuestion,
                Scoring = new List<Scoring>
                        {
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectResult,
                                Points = 1
                            }
                        },
                CompetitionId = 1,
                TargetId = 1
            };

            Tools.CreateTestData(
                _context,
                participators: new List<Participator>
                {
                    new Participator
                    {
                        Competition = 1,
                        UserId = 1,
                        Role = Enums.CompetitionRole.Host
                    }
                },
                targets: new List<Target>
                {
                    updateTarget
                });

            // Mark entry state as detached so update works.
            _context.Entry(updateTarget).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var jArray = CompetitionPage.TargetsToJArray(
                new List<Target>
                {
                    new Target
                    {
                        TargetId = 1,
                        Bet = "Updated",
                        Type = Enums.TargetType.OpenQuestion,
                        Scoring = new List<Scoring>
                        {
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectResult,
                                Points = 1
                            }
                        }
                    },
                    new Target
                    {
                        Bet = "Bet",
                        Type = Enums.TargetType.Result,
                        Scoring = new List<Scoring>
                        {
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectResult,
                                Points = 2
                            },
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectWinner,
                                Points = 1
                            }
                        }
                    },
                    new Target
                    {
                        Type = Enums.TargetType.Selection,
                        Bet = "Bet",
                        Scoring = new List<Scoring>
                        {
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectResult,
                                Points = 2
                            }
                        },
                        Selections = new List<string>
                        {
                            "Selection 1",
                            "Selection 2"
                        }
                    }
                });

            var action = new PageAction(
                userId: 1,
                page: "test",
                action: "SaveBetTargets",
                parameters: new Dictionary<string, object>
                {
                    { "betTargets", (object)jArray }
                },
                pageId: 1);

            var response = _competitionPage.HandleAction(action);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var competitionTargets = _targetAction.GetTargets(competitionId: 1);
            Assert.AreEqual(3, competitionTargets.Count);
            Assert.AreEqual(1, competitionTargets.Count(target => target.Bet == "Updated"));

            _context.Target.RemoveRange(_context.Target);
            _context.Participator.RemoveRange(_context.Participator);
        }
    }
}
