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
        public void Post([FromBody] string folder = null)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/Folders/5
        public void Delete(string folder)
        {
            // TODO: folder osoitteesta vai querystä?
            throw new NotImplementedException();
        }
    }
}
