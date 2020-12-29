using Api.Classes;
using Betkeeper.Classes;
using Betkeeper.Actions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BetsController : ApiController
    {
        private BetAction BetAction { get; }

        public BetsController()
        {
            BetAction = new BetAction();
        }

        // GET: api/Bets
        /// <summary>
        /// Gets bets.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="finished">Should only finished, not finished, or all bets be returned</param>
        /// <returns></returns>
        public HttpResponseMessage Get(
            [FromUri]bool? finished = null,
            [FromUri]string folder = null)
        {
            var userId = TokenLog.GetUserIdFromRequest(Request);
            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized, "Session expired", Http.ContentType.Text);
            }

            var bets = BetAction.GetBets(userId.Value, finished, folder);

            return Http.CreateResponse(HttpStatusCode.OK, bets);
        }
    }
}
