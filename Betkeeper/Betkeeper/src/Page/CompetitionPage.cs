using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Betkeeper.Classes;
using Betkeeper.Actions;
using Betkeeper.Extensions;

namespace Betkeeper.Page
{
    /// <summary>
    /// A competition page structure.
    /// </summary>
    public class CompetitionPage: Page
    {
        protected CompetitionAction CompetitionAction { get; set; }

        public CompetitionPage()
        {
            CompetitionAction = new CompetitionAction();
        }

        public override PageResponse GetResponse(string pageKey, int userId)
        {
            throw new NotImplementedException();
        }

        public override HttpResponseMessage HandleAction(PageAction action)
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
            // TODO: Validoi arvot
            var competitionId = action.Parameters.GetInt("CompetitionId");

            if (competitionId == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest, "Missing CompetitionId parameter");
            }

            try
            {
                CompetitionAction.DeleteCompetition(action.UserId, (int)competitionId);

                return Http.CreateResponse(HttpStatusCode.NoContent, "Competition deleted successfully");
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
