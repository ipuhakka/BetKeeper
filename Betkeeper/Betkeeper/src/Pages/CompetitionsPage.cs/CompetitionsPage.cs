using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Page.Components;
using Betkeeper.Page;
using Betkeeper.Extensions;
using Betkeeper.Exceptions;

namespace Betkeeper.Pages
{
    public class CompetitionsPage: IPage
    {
        protected CompetitionAction CompetitionAction { get; set; }

        public CompetitionsPage()
        {
            CompetitionAction = new CompetitionAction();
        }

        public HttpResponseMessage GetResponse(string pageKey, int userId)
        {
            var components = new List<Component>
            {
                new Table(
                    "competitions", 
                    new List<DataField>
                    {
                        new DataField("name", DataType.String),
                        new DataField("startTime", DataType.DateTime),
                        new DataField("state", DataType.String)
                    },
                    "competitionId"),
                new ModalActionButton(
                    "JoinCompetition",
                    new List<Component>
                    {
                        new Field("JoinCode", "Join code", FieldType.TextBox)
                    },
                    "Join competition"),
                new ModalActionButton(
                    "Post",
                    new List<Component>
                    {
                        new Field("Name", "Name", FieldType.TextBox),
                        new Field("StartTime", "Start time", FieldType.DateTime),
                        new Field("Description", "Description", FieldType.TextArea)
                    },
                    "Create a competition")
            };

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

                case "JoinCompetition":
                    return JoinCompetition(action);

                default:
                    throw new NotImplementedException();
            }
        }

        public HttpResponseMessage HandleDropdownUpdate(
            Dictionary<string, object> data, 
            int? pageId = null)
        {
            throw new NotImplementedException();
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

        private HttpResponseMessage JoinCompetition(PageAction action)
        {
            var joinCode = action.Parameters.GetString("JoinCode");

            if (string.IsNullOrEmpty(joinCode))
            {
                return Http.CreateResponse(
                    HttpStatusCode.BadRequest,
                    "Join code empty");
            }

            try
            {
                CompetitionAction.JoinCompetition(joinCode, action.UserId);
            }
            catch (InvalidOperationException)
            {
                return Http.CreateResponse(
                    HttpStatusCode.Conflict,
                    "Competition has already started and does not accept new players");
            }
            catch (NotFoundException)
            {
                return Http.CreateResponse(
                    HttpStatusCode.NotFound,
                    "Join code did not match any competition");
            }

            return Http.CreateResponse(
                    HttpStatusCode.OK,
                    "Joined competition successfullyCo");
        }
    }
}
