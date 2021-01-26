using Api.Classes;
using Betkeeper.Classes;
using Betkeeper.Page;
using Betkeeper.Services;
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

        [Route("api/page/handleDropdownUpdate/{page}")]
        public HttpResponseMessage HandleDropdownUpdate([FromUri]string page)
        {
            return DropdownUpdate(page);
        }

        [Route("api/page/handleDropdownUpdate/{page}/{id}")]
        public HttpResponseMessage HandleDropdownUpdate([FromUri]string page, int id)
        {
            return DropdownUpdate(page, id);
        }

        [Route("api/page/expandListGroupItem/{page}")]
        public HttpResponseMessage ExpandListGroupItem([FromUri]string page)
        {
            return HandleExpandListGroupItem(page);
        }

        private HttpResponseMessage DropdownUpdate(string page, int? id = null)
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

            return Http.CreateResponse(
                HttpStatusCode.OK,
                PageService
                    .GetPageInstance(page)
                    .HandleDropdownUpdate(new DropdownUpdateParameters((int)userId, parameters, id)));
        }
        
        private HttpResponseMessage HandleExpandListGroupItem(string page)
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

            return Http.CreateResponse(
                HttpStatusCode.OK,
                PageService
                    .GetPageInstance(page)
                    .ExpandListGroupItem(new ListGroupItemExpandParameters((int)userId, Http.GetContentAsDictionary(Request))));
        }
    }
}
