using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Betkeeper.Classes;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PageController : ApiController
    {
        // GET: api/Page
        [Route("api/page/{page}")]
        public HttpResponseMessage Get([FromUri]string page)
        {
            return Http.CreateResponse(HttpStatusCode.OK);
        }
    }
}
