using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Api.Classes;
using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Extensions;

namespace Api.Controllers
{
    public class CompetitionsController : ApiController
    {
        protected CompetitionAction CompetitionAction { get; set; }

        public CompetitionsController()
        {
            CompetitionAction = new CompetitionAction();
        }

        // GET: api/Competitions
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Competitions/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Competitions
        /// <summary>
        /// Create a new competition.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Post()
        {
            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var data = Http.GetContentAsDictionary(Request);

            var startTime = data.GetDateTime("startTime");

            if (startTime == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var name = data.GetString("name");

            if (string.IsNullOrEmpty(name))
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            CompetitionAction.CreateCompetition(
                (int)userId,
                name,
                data.GetString("description"),
                (DateTime)startTime);

            return Http.CreateResponse(HttpStatusCode.OK);
        }

        // PUT: api/Competitions/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Competitions/5
        public void Delete(int id)
        {
        }
    }
}
