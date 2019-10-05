using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Betkeeper.Data;
using Betkeeper.Repositories;
using Api.Classes;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FoldersController : ApiController
    {
        public IFolderRepository _FolderRepository;

        // GET: api/Folders
        public HttpResponseMessage Get([FromUri] int? betId = null)
        {
            _FolderRepository = _FolderRepository ?? new FolderRepository();

            var tokenString = Request.Headers.Authorization?.ToString();

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var folders = _FolderRepository.GetUsersFolders(
                userId: (int)userId,
                betId: betId);

            return Http.CreateResponse(HttpStatusCode.OK, data: folders);
        }

        // POST: api/Folders
        public HttpResponseMessage Post([FromBody] string folder)
        {
            _FolderRepository = _FolderRepository ?? new FolderRepository();

            if (folder == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (_FolderRepository.UserHasFolder((int)userId, folder))
            {
                return Http.CreateResponse(HttpStatusCode.Conflict);
            }

            _FolderRepository.AddNewFolder((int)userId, folder);

            return Http.CreateResponse(HttpStatusCode.Created);
        }

        // DELETE: api/Folders/folderToDelete
        [Route("api/folders/{folder}")]
        public HttpResponseMessage Delete([FromUri]string folder)
        {
            // TODO: Folder ei toimi, id toimii
            _FolderRepository = _FolderRepository ?? new FolderRepository();

            var tokenString = Request.Headers.Authorization?.ToString();

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            if (!_FolderRepository.UserHasFolder((int)userId, folder))
            {
                return Http.CreateResponse(HttpStatusCode.NotFound);
            }

            _FolderRepository.DeleteFolder((int)userId, folder);

            return Http.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
