﻿using Api.Classes;
using Betkeeper.Classes;
using Betkeeper.Page;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PageController : ApiController
    {
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

        [Route("api/page/{page}/{id}")]
        public HttpResponseMessage Get([FromUri]string page, int id)
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

            return PageResponse.GetResponseMessage(page, (int)userId, id);
        }

        [Route("api/page/handleDropdownUpdate/{page}/{id}")]
        public HttpResponseMessage HandeDropdownUpdate([FromUri]string page, int? id)
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

            return PageResponse
                .GetPageInstance(page, id)
                .HandleDropdownUpdate(
                    parameters,
                    id
                );
        }
    }
}
