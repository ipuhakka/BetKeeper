using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Models;
using Betkeeper.Page;
using Betkeeper.Pages.CompetitionPage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            _competitionPage = new CompetitionPage(_competitionAction, _targetAction);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _competitionPage.Dispose();
        }

        [Test]
        public void SaveBetTargets_NoQuestionGiven_ReturnsBadRequest()
        {
            var testTargetsJArray = CompetitionPage.TargetsToJArray(new List<Target>
            {
                new Target
                {
                    Bet = null
                }
            });

            var action = new PageAction(
                userId: 1,
                page: "test",
                action: "SaveBetTargets",
                parameters: new Dictionary<string, object>
                {
                    { "betTargets", (object)testTargetsJArray }
                },
                pageId: 1);

            var response = _competitionPage.HandleAction(action);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            // First error validated
            var actionResponse = Http.GetHttpContent<PageActionResponse>(response.Content);
            Assert.AreEqual("Row 1: No question given", actionResponse.Message);
        }
        
        [Test]
        public void SaveBetTargets_NoResultPointsGiven_ReturnsBadRequest()
        {
            var testTargetsJArray = CompetitionPage.TargetsToJArray(new List<Target>
            {
                new Target
                {
                    Bet = "Question"
                }
            });

            var action = new PageAction(
                userId: 1,
                page: "test",
                action: "SaveBetTargets",
                parameters: new Dictionary<string, object>
                {
                    { "betTargets", (object)testTargetsJArray }
                },
                pageId: 1);

            var response = _competitionPage.HandleAction(action);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            // First error validated
            var actionResponse = Http.GetHttpContent<PageActionResponse>(response.Content);
            Assert.AreEqual(
                "Row 1: Missing points for correct result", 
                actionResponse.Message);
        }

        [Test]
        public void SaveBetTargets_NoCorrectWinnerPointsGiven_ReturnsBadRequest()
        {
            var testTargetsJArray = CompetitionPage.TargetsToJArray(new List<Target>
            {
                new Target
                {
                    Bet = "Question",
                    Scoring = new List<Scoring>
                    {
                        new Scoring
                        {
                            Score = Enums.TargetScore.CorrectResult,
                            Points = 1
                        }
                    }
                }
            });

            var action = new PageAction(
                userId: 1,
                page: "test",
                action: "SaveBetTargets",
                parameters: new Dictionary<string, object>
                {
                    { "betTargets", (object)testTargetsJArray }
                },
                pageId: 1);

            var response = _competitionPage.HandleAction(action);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            // First error validated
            var actionResponse = Http.GetHttpContent<PageActionResponse>(response.Content);
            Assert.AreEqual(
                "Row 1: Missing points for correct winner",
                actionResponse.Message);
        }

        [Test]
        public void SaveBetTargets_SelectionBetWithNoSelections_ReturnsBadRequest()
        {
            var jArrays = new List<List<Target>>
            {
                new List<Target>
                {
                    new Target
                    {
                        Bet = "Question",
                        Scoring = new List<Scoring>
                        {
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectResult,
                                Points = 1
                            },
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectWinner,
                                Points = 0.5
                            }
                        },
                        Type = Enums.TargetType.Selection,
                        Selections = null
                    }
                },
                new List<Target>
                {
                    new Target
                    {
                        Bet = "Question",
                        Scoring = new List<Scoring>
                        {
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectResult,
                                Points = 1
                            },
                            new Scoring
                            {
                                Score = Enums.TargetScore.CorrectWinner,
                                Points = 0.5
                            }
                        },
                        Type = Enums.TargetType.Selection,
                        Selections = new List<string>()
                    }
                }
            }.Select(targetList => CompetitionPage.TargetsToJArray(targetList))
            .ToList();

            jArrays.ForEach(jArray =>
            {
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
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

                // First error validated
                var actionResponse = Http.GetHttpContent<PageActionResponse>(response.Content);
                Assert.AreEqual(
                    "Row 1: No selections given for selection typed bet",
                    actionResponse.Message);
            });
        }

        [Test]
        public void SaveBetTargets_ValidTargetsAreCreated()
        {
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
                });

            var jArray = CompetitionPage.TargetsToJArray(
                new List<Target>
                {
                    new Target
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

            Assert.AreEqual(3, _targetAction.GetTargets(competitionId: 1).Count);

            _context.Target.RemoveRange(_context.Target);
            _context.Participator.RemoveRange(_context.Participator);
        }
    }
}
