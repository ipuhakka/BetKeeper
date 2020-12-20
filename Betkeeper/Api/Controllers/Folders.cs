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
    public class FoldersController : ApiController
    {
        public FolderAction FolderAction;

        public FoldersController()
        {
            FolderAction = new FolderAction();
        }

        // GET: api/Folders
        public HttpResponseMessage Get([FromUri] int? betId = null)
        {
            FolderAction = new FolderAction();

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var folders = FolderAction.GetUsersFolders(
                userId: (int)userId,
                betId: betId);

            return Http.CreateResponse(HttpStatusCode.OK, data: folders);
        }
    }
}
