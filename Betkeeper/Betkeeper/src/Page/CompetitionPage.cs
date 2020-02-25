using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Page.Components;
using Betkeeper.Extensions;
using Betkeeper.Exceptions;

namespace Betkeeper.Page
{
    public class CompetitionPage
    {
        protected CompetitionAction CompetitionAction { get; set; }

        public CompetitionPage()
        {
            CompetitionAction = new CompetitionAction();
        }

        public HttpResponseMessage GetResponse(string pageKey)
        {
            var components = new List<Component>
            {
                new ModalActionButton(
                    "Post", 
                    new List<Field>
                    {
                        new Field("Name", "Name", FieldType.TextBox),
                        new Field("StartTime", "Start time", FieldType.DateTime),
                        new Field("Description", "Description", FieldType.TextArea),
                        new Field("TestInt", "Integer field test", FieldType.Integer),
                        new Field("TestDouble", "Double field test", FieldType.Double)
                    },
                    "Create a competition"),
                new NavigationButton(
                    "/page/competitions/1", 
                    "Check out this competition", 
                    "outline-primary")
            };

            // TODO: Hae data.
            var dataDictionary = new Dictionary<string, object>();

            dataDictionary.Add("Competitions", CompetitionAction.GetUsersCompetitions(userId));

            return Http.CreateResponse(
                HttpStatusCode.OK, 
                new PageResponse(pageKey, components, dataDictionary));
        }

        public HttpResponseMessage HandleAction(PageAction action)
        {
            switch (action.ActionName)
            {
                case "Post":
                    return Post(action);

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        ///  Adds a new competition.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private HttpResponseMessage Post(PageAction action)
        {
            var startTime = action.Parameters.GetDateTime("StartTime");

            if (startTime == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var name = action.Parameters.GetString("Name");

            if (string.IsNullOrEmpty(name))
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                CompetitionAction.CreateCompetition(
                    action.UserId,
                    name,
                    action.Parameters.GetString("Description"),
                    (DateTime)startTime);
            }
            catch (NameInUseException)
            {
                return Http.CreateResponse(
                    HttpStatusCode.Conflict,
                    "A tournament with specified name already exists");
            }

            return Http.CreateResponse(HttpStatusCode.Created, "Competition created successfully");
        }
    }
}
