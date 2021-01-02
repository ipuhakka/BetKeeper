using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Exceptions;
using Betkeeper.Extensions;
using Betkeeper.Enums;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Betkeeper.Pages
{
    public class CompetitionsPage : PageBase
    {
        private CompetitionAction CompetitionAction { get; set; }

        public CompetitionsPage()
        {
            CompetitionAction = new CompetitionAction();
        }

        public CompetitionsPage(CompetitionAction competitionAction)
        {
            CompetitionAction = competitionAction;
        }

        public override PageResponse GetPage(string pageKey, int userId)
        {
            var components = new List<Component>
            {
                new Table(
                    "competitions",
                    new List<ItemField>
                    {
                        new ItemField("name", DataType.String),
                        new ItemField("startTime", DataType.DateTime),
                        new ItemField("state", DataType.String)
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

            var dataDictionary = new Dictionary<string, object>
            {
                { "Competitions", CompetitionAction.GetUsersCompetitions(userId) }
            };

            return new PageResponse(pageKey, components, dataDictionary);
        }

        public override PageActionResponse HandleAction(PageAction action)
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

        /// <summary>
        ///  Adds a new competition.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse Post(PageAction action)
        {
            var startTime = action.Parameters.GetDateTime("StartTime");

            if (startTime == null)
            {
                return new PageActionResponse(ActionResultType.InvalidInput);
            }

            var name = action.Parameters.GetString("Name");

            if (string.IsNullOrEmpty(name))
            {
                return new PageActionResponse(ActionResultType.InvalidInput);
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
                return new PageActionResponse(
                    ActionResultType.Conflict,
                    "A tournament with specified name already exists");
            }

            return new PageActionResponse(
                ActionResultType.Created,
                "Competition created successfully",
                refresh: true);
        }

        private PageActionResponse JoinCompetition(PageAction action)
        {
            var joinCode = action.Parameters.GetString("JoinCode");

            if (string.IsNullOrEmpty(joinCode))
            {
                return new PageActionResponse(
                    ActionResultType.InvalidInput,
                    "Join code empty");
            }

            CompetitionAction.JoinCompetition(joinCode, action.UserId);

            return new PageActionResponse(
                ActionResultType.OK,
                "Joined competition successfully",
                refresh: true);
        }
    }
}
