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

        // POST: api/Folders
        public HttpResponseMessage Post([FromBody]string folder)
        {
            if (folder == null || folder.Length > 50)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (FolderAction.UserHasFolder((int)userId, folder))
            {
                return Http.CreateResponse(HttpStatusCode.Conflict);
            }

            FolderAction.AddFolder((int)userId, folder);

            return Http.CreateResponse(HttpStatusCode.Created);
        }

        // DELETE: api/Folders/folderToDelete
        [Route("api/folders/{folder}")]
        public HttpResponseMessage Delete([FromUri]string folder)
        {
            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            try
            {
                FolderAction.DeleteFolder((int)userId, folder);
            }
            catch (ActionException actionException)
            {
                return Http.CreateResponse(
                    (HttpStatusCode)actionException.ActionExceptionType);
            }

            return Http.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
