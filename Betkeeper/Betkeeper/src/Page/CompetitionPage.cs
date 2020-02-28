using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Betkeeper.Classes;
using Betkeeper.Actions;
using Betkeeper.Extensions;
using Betkeeper.Page.Components;

namespace Betkeeper.Page
{
    /// <summary>
    /// A competition page structure.
    /// </summary>
    public class CompetitionPage: IPage
    {
        protected CompetitionAction CompetitionAction { get; set; }

        public CompetitionPage()
        {
            CompetitionAction = new CompetitionAction();
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

            var components = new List<Component>
            {
                new Field("joinCode", "Join code", true, FieldType.TextBox, "competition.joinCode")
            };

            if (participator.Role == (int)Enums.CompetitionRole.Host)
            {
                components.Add(new PageActionButton(
                    "DeleteCompetition",
                    new List<string> { "competitionId" },
                    "Delete competition",
                    "outline-danger",
                    requireConfirm: true,
                    navigateTo: "../competitions")
                );
            }

            var data = new Dictionary<string, object>();

            data.Add("CompetitionId", competitionId);
            data.Add("Competition", CompetitionAction.GetCompetition(competitionId));

            return Http.CreateResponse(
                HttpStatusCode.OK,
                new PageResponse($"competitions/{pageId}", components, data));
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
            }
        }

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

                return Http.CreateResponse(HttpStatusCode.OK, "Competition deleted successfully");
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
