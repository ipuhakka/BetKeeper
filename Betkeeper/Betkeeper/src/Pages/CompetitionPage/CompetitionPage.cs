using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Extensions;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Betkeeper.Pages
{
    /// <summary>
    /// A competition page structure.
    /// </summary>
    public partial class CompetitionPage : IPage, IDisposable
    {
        private CompetitionAction CompetitionAction { get; set; }

        public CompetitionPage()
        {
            CompetitionAction = new CompetitionAction();
        }

        public CompetitionPage(CompetitionAction competitionAction)
        {
            CompetitionAction = competitionAction;
        }

        public void Dispose()
        {
            CompetitionAction.Dispose();
        }

        public HttpResponseMessage GetResponse(string pageId, int userId)
        {
            var competitionId = int.Parse(pageId);

            var participator = CompetitionAction.GetParticipator(userId, competitionId);

            // User does not have rights to competition, redirect
            if (participator == null)
            {
                var response = Http.CreateResponse(HttpStatusCode.Redirect);
                response.Headers.Add("Location", "../competitions/");

                return response;
            }

            // Yleinen näkymä, osallistujat, vedot, hostille kilpailun hallinta
            var tabs = new List<Component>();

            // General view
            tabs.Add(new Tab(
                "home",
                "Home",
                new List<Component>
                {
                    new Field("name", "Name", true, FieldType.TextBox, "competition.name"),
                    new Field("joinCode", "Join code", true, FieldType.TextBox, "competition.joinCode"),
                    new Container(
                        children: new List<Component>
                        {
                            new Dropdown(
                                "test",
                                "Testlabel",
                                new List<Option>
                                {
                                    new Option("0", "Rainbow"),
                                    new Option("1", "Dark", initialValue: true)
                                },
                                "test",
                                componentsToUpdate: new List<string>{ "testUpdateContainer" }
                            ),
                            new Dropdown(
                                "test2",
                                "test 2 label",
                                new List<Option>
                                {
                                    new Option("0", "Marshmellows"),
                                    new Option("1", "Chocolate"),
                                    new Option("2", "Whipped cream"),
                                    new Option("3", "Fudge")
                                }
                            )
                        },
                        componentKey: "testUpdateContainer")
                }));

            // Results
            tabs.Add(GetResultsTab(competitionId));

            //Bets
            tabs.Add(new Tab(
                "bets",
                "Bets",
                new List<Component>
                {

                }));

            if (participator.Role == CompetitionRole.Host)
            {
                tabs.Add(GetManageBetsTab());

                tabs.Add(new Tab(
                    "settings",
                    "Settings",
                    new List<Component>
                    {
                        new PageActionButton(
                            "DeleteCompetition",
                            new List<string> { "competitionId" },
                            "Delete competition",
                            "outline-danger",
                            requireConfirm: true,
                            navigateTo: "../competitions")
                    }));
            }


            var data = new Dictionary<string, object>();

            data.Add("CompetitionId", competitionId);
            data.Add("Competition", CompetitionAction.GetCompetition(competitionId));

            return Http.CreateResponse(
                HttpStatusCode.OK,
                new PageResponse($"competitions/{pageId}", tabs, data));
        }

        public HttpResponseMessage HandleAction(PageAction action)
        {
            switch (action.ActionName)
            {
                default:
                    return Http.CreateResponse(
                        HttpStatusCode.NotFound,
                        "Action not found");

                case "DeleteCompetition":
                    return DeleteCompetition(action);

                case "AddBetContainer":
                    return AddBetContainer(action);

                case "CancelBetTargetsUpdate":
                    return CancelBetTargetsUpdate(action);
            }
        }

        public HttpResponseMessage HandleDropdownUpdate(
            Dictionary<string, object> data,
            int? pageId = null)
        {
            var componentKey = data.GetString("key");
            var value = data.GetString("value");

            if (data.ContainsKey("components"))
            {
                var components = ComponentParser.ParseComponents(data["components"].ToString());

                if (componentKey == "test")
                {
                    var options = new List<Option>
                        {
                            new Option("0", "Marshmellows"),
                            new Option("1", "Chocolate")
                        };

                    if (value == "1")
                    {
                        options.Add(new Option("2", "Whipped cream"));
                        options.Add(new Option("3", "Fudge"));
                    }

                    var newContainer = new Container(
                        children: new List<Component>
                        {
                            new Dropdown(
                                "test",
                                "Testlabel",
                                new List<Option>
                                {
                                    new Option("0", "Rainbow"),
                                    new Option("1", "Dark")
                                },
                                "test",
                                componentsToUpdate: new List<string>{ "testUpdateContainer" }
                            ),
                            new Dropdown(
                                "test2",
                                "test 2 label",
                                options
                            )
                        },
                        componentKey: "testUpdateContainer");

                    return Http.CreateResponse(
                        HttpStatusCode.OK,
                        new PageResponse(new List<Component> { newContainer }));
                }

                if (componentKey.Contains("bet-type-"))
                {
                    var newTargetType = (TargetType)Enum.Parse(typeof(TargetType), value, true);

                    // Get index, format of component key is bet-type-{index}
                    var index = int.Parse(componentKey.Split('-').Last());

                    return Http.CreateResponse(
                        HttpStatusCode.OK,
                        new PageResponse(
                            new List<Component>
                            {
                                CreateTargetContainer(index, newTargetType)
                            })
                        );
                }
            }

            throw new ArgumentException($"{componentKey} options update not implemented");
        }

        private Tab GetResultsTab(int competitionId)
        {

            // TODO: Käyttäjätaulu entitymalliin
            var components = new List<Component>();

            //components.Add(new Table(
            //    "participators",
            //    new List<DataField>
            //    {
            //    }));

            return new Tab("participators", "Participators", components);
        }

        //private Tab GetBetsTab(Competition competition, Participator participator)
        //{
        //    var components = new List<Component>();

        //    if (participator.Role == (int)CompetitionRole.Host)
        //    {
        //        components.Add(new ModalActionButton("AddTargets", new List<Field>(), "Add targets"))
        //    }
        //}

        private HttpResponseMessage DeleteCompetition(PageAction action)
        {
            var competitionId = action.Parameters.GetInt("competitionId");

            if (competitionId == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest, "Missing CompetitionId parameter");
            }

            try
            {
                CompetitionAction.DeleteCompetition(action.UserId, (int)competitionId);

                return Http.CreateResponse(
                    HttpStatusCode.OK,
                    new PageActionResponse("Competition deleted successfully"));
            }
            catch (InvalidOperationException)
            {
                return Http.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    "User not allowed to delete competition");
            }
        }
    }
}
