using Api.Classes;
using Betkeeper.Classes;
using Betkeeper.Page;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PageActionController : ApiController
    {
        /// <summary>
        /// Executes a page action.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [Route("api/pageaction/{page}/{pageaction}")]
        public HttpResponseMessage Action(string page, string pageAction)
        {
            if (string.IsNullOrEmpty(page))
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var parameters = Http.GetContentAsDictionary(Request);

            return PageResponse.HandlePageAction(
                new PageAction((int)userId, page, pageAction, parameters));
        }

        /// <summary>
        /// Executes a page action.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [Route("api/pageaction/{page}/{pageId}/{pageAction}")]
        public HttpResponseMessage Action(string page, int pageId, string pageAction)
        {
            if (string.IsNullOrEmpty(page))
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var parameters = Http.GetContentAsDictionary(Request);

            return PageResponse.HandlePageAction(
                new PageAction((int)userId, page, pageAction, parameters, pageId));
        }
    }
}
