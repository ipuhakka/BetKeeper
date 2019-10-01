using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Betkeeper.Data;
using Betkeeper.Models;
using Api.Classes;

namespace Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FoldersController : ApiController
    {
        public IFolderModel _FolderModel;

        // GET: api/Folders
        public HttpResponseMessage Get([FromUri] int? betId = null)
        {
            _FolderModel = _FolderModel ?? new FolderModel();

            var tokenString = Request.Headers.Authorization?.ToString();

            if (tokenString == null 
                || !TokenLog.ContainsToken(tokenString))
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var folders = _FolderModel.GetUsersFolders(
                userId: (int)TokenLog.GetTokenOwner(tokenString),
                betId: betId);

            return Http.CreateResponse(HttpStatusCode.OK, data: folders);
        }

        // POST: api/Folders
        public HttpResponseMessage Post([FromBody] string folder)
        {
            _FolderModel = _FolderModel ?? new FolderModel();

            if (folder == null)
            {
                return Http.CreateResponse(HttpStatusCode.BadRequest);
            }

            var tokenString = Request.Headers.Authorization?.ToString();

            if (tokenString == null
                || !TokenLog.ContainsToken(tokenString))
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }       

            var userId = (int)TokenLog.GetTokenOwner(tokenString);

            if (_FolderModel.UserHasFolder(userId, folder))
            {
                return Http.CreateResponse(HttpStatusCode.Conflict);
            }

            _FolderModel.AddNewFolder(userId, folder);

            return Http.CreateResponse(HttpStatusCode.Created);
        }

        // DELETE: api/Folders/folderToDelete
        [Route("api/folders/{folder}")]
        public HttpResponseMessage Delete([FromUri]string folder)
        {
            // TODO: Folder ei toimi, id toimii
            _FolderModel = _FolderModel ?? new FolderModel();

            var tokenString = Request.Headers.Authorization?.ToString();

            if (tokenString == null
                || !TokenLog.ContainsToken(tokenString))
            {
                return Http.CreateResponse(HttpStatusCode.Unauthorized);
            }

            var userId = (int)TokenLog.GetTokenOwner(tokenString);

            if (!_FolderModel.UserHasFolder(userId, folder))
            {
                return Http.CreateResponse(HttpStatusCode.NotFound);
            }

            _FolderModel.DeleteFolder(userId, folder);

            return Http.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
