using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Classes;
using Betkeeper.Classes;
using Betkeeper.Page;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PageController : ApiController
    {
        // GET: api/Page
        [Route("api/page/{page}")]
        public HttpResponseMessage Get([FromUri]string page)
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

            return PageResponse.GetResponseMessage(page, (int)userId);
        }
    }
}
