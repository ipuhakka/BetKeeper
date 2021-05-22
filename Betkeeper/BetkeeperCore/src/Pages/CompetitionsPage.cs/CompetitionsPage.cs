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
using System.Linq;

namespace Betkeeper.Pages
{
    public class CompetitionsPage : PageBase
    {
        public override string PageKey => "competitions";

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
            var invitations = CompetitionInvitationAction.GetUsersInvitations(userId);

            var invitationContainers = invitations
                .Select(invitation => new Container(
                    new List<Component>
                    {
                        new Label(invitation.CompetitionName),
                        new Label(invitation.StartTime.ToString("u"), isDate: true),
                        new PageActionButton(
                            action: "DeclineInvitation",
                            actionDataKeys: new List<string>(),
                            text: "Decline",
                            buttonStyle: "danger",
                            requireConfirm: true,
                            componentsToInclude: new List<string>(),
                            displayType: DisplayType.Icon,
                            staticData: new Dictionary<string, object>
                            {
                                { "invitationId", invitation.InvitationId }
                            })
                        {
                            IconName = "fas fa-times"
                        },
                        new PageActionButton(
                            action: "AcceptInvitation",
                            actionDataKeys: new List<string>(),
                            text: "Accept",
                            buttonStyle: "success",
                            requireConfirm: true,
                            componentsToInclude: new List<string>(),
                            displayType: DisplayType.Icon,
                            staticData: new Dictionary<string, object>
                            {
                                { "invitationId", invitation.InvitationId }
                            })
                        {
                            IconName = "fas fa-check"
                        }
                    })
                { 
                    CustomCssClass = "competitions-invitation-container"
                })
                .Cast<Component>()
                .ToList();

            var invitationsContainer = new Container(invitationContainers);

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
                    "competition",
                    "competitionId"),
                new ModalActionButton(
                    "JoinCompetition",
                    new List<Component>
                    {
                        new Field("JoinCode", "Join code", FieldType.TextBox),
                        invitationsContainer
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
                { "competitions", CompetitionAction.GetUsersCompetitions(userId) }
            };

            return new PageResponse(pageKey, components, dataDictionary);
        }

        public override PageActionResponse HandleAction(PageAction action)
        {
            return action.ActionName switch
            {
                "Post" => Post(action),
                "JoinCompetition" => JoinCompetition(action),
                "DeclineInvitation" => DeclineInvitation(action),
                "AcceptInvitation" => AcceptInvitation(action),
                _ => throw new NotImplementedException(),
            };
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

        /// <summary>
        /// Decline an invitation to competition
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static PageActionResponse DeclineInvitation(PageAction action)
        {
            var invitation = (int)action.Parameters.GetInt("invitationId");

            CompetitionInvitationAction.DeclineInvitation(action.UserId, invitation);

            return new PageActionResponse(ActionResultType.OK, "Invitation declined", refresh: true);
        }

        /// <summary>
        /// Accept invitation to competition
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static PageActionResponse AcceptInvitation(PageAction action)
        {
            var invitation = (int)action.Parameters.GetInt("invitationId");

            CompetitionInvitationAction.AcceptInvitation(action.UserId, invitation);

            return new PageActionResponse(ActionResultType.OK, "Invitation accepted", refresh: true);
        }
    }
}
