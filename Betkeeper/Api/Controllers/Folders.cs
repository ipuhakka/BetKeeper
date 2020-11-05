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
            if (string.IsNullOrEmpty(folder) || folder.Length > 50)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest, 
                    string.IsNullOrEmpty(folder) 
                        ? "Folder name cannot be empty"
                        : "Folder name cannot exceed 50 characters", 
                    Http.ContentType.Text);
            }

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (FolderAction.UserHasFolder((int)userId, folder))
            {
                return Http.CreateResponse(
                    HttpStatusCode.Conflict, 
                    "User already has folder with same name", 
                    Http.ContentType.Text);
            }

            FolderAction.AddFolder((int)userId, folder);

            return Http.CreateResponse(
                HttpStatusCode.Created, 
                "Folder created successfully",
                Http.ContentType.Text);
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
                    (HttpStatusCode)actionException.ActionExceptionType,
                    actionException.Message);
            }

            return Http.CreateResponse(
                HttpStatusCode.NoContent, 
                "Deleted successfully",
                Http.ContentType.Text);
        }
    }
}
