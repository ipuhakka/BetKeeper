using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using API.Models;
using BetKeeper.Exceptions;
using Newtonsoft.Json;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FoldersController: ApiController
    {
        /// <summary>
        /// Returns a list of users bet folders names.
        /// 
        /// Responses:
        ///     200 OK
        ///     401 Unauthorized    
        /// </summary>
        public HttpResponseMessage Get([FromUri] int bet_id = -1)
        {
            if (Request.Headers.Authorization == null || !TokenLog.ContainsToken(Request.Headers.Authorization.ToString()))
            {
                return HttpMessage.ResponseMessage("Request did not contain a valid token", HttpStatusCode.Unauthorized);
            }

            try
            {
                FoldersModel model = new FoldersModel();
                string data = model.GetUsersFolders(Request.Headers.Authorization.ToString(), bet_id);
                return HttpMessage.ResponseMessage(data, HttpStatusCode.OK, "application/json");
            }
            catch (AuthenticationError e)
            {
                return HttpMessage.ResponseMessage(e.ErrorMessage, HttpStatusCode.Unauthorized);
            }        
        } 
        
        /// <summary>
        /// Adds a new row to table 'bet_folders'.
        /// 
        /// Responses:
        ///     201 Created
        ///     400 Bad request
        ///     401 Unauthorized
        ///     409 Conflict (user already has folder of same name)
        /// </summary>
        public HttpResponseMessage Post()
        {
            dynamic data;
            HttpContent requestContent = Request.Content;
            string jsonContent = requestContent.ReadAsStringAsync().Result;
            try
            {
                data = JsonConvert.DeserializeObject(jsonContent);
            }
            catch (JsonReaderException)
            {
                return HttpMessage.ResponseMessage("Request body was malformed", HttpStatusCode.BadRequest);
            }

            if (Request.Headers.Authorization == null || !TokenLog.ContainsToken(Request.Headers.Authorization.ToString()))
            {
                return HttpMessage.ResponseMessage("Request did not contain a valid token", HttpStatusCode.Unauthorized);
            }
            if (data["folder"] == null)
            {
                return HttpMessage.ResponseMessage("Request body did not contain folder attribute", HttpStatusCode.BadRequest);
            }
            else
            {
                FoldersModel model = new FoldersModel();
                int res = 0;
                try
                {
                    res = model.AddFolder(Request.Headers.Authorization.ToString(), data["folder"].ToString());
                }
                catch (AuthenticationError)
                {
                    return HttpMessage.ResponseMessage("Token did not belong to any user", HttpStatusCode.Unauthorized);
                }
                if (res == -1)
                {
                    return HttpMessage.ResponseMessage("Creation failed: resource already exists", HttpStatusCode.Conflict);
                }
                else
                {
                    return HttpMessage.ResponseMessage("Created folder successfully", HttpStatusCode.Created);
                }
            }
        }

        /// <summary>
        /// Deletes a row from table 'bet_folders'. Deleted folder is specified in uri query part 'api/folders?folder={folder}'.
        /// 
        /// Responses:
        ///     
        /// </summary>
        public HttpResponseMessage Delete(string folder = null)
        {
            if (Request.Headers.Authorization == null || !TokenLog.ContainsToken(Request.Headers.Authorization.ToString()))
            {
                return HttpMessage.ResponseMessage("Request did not contain a valid token", HttpStatusCode.Unauthorized);
            }

            if (folder == null)
            {
                return HttpMessage.ResponseMessage("Request contained no resource to delete", HttpStatusCode.BadRequest);
            }
            
            try
            {
                FoldersModel model = new FoldersModel();
                int res = model.DeleteFolder(Request.Headers.Authorization.ToString(), folder);

                if (res == 0)
                {
                    return HttpMessage.ResponseMessage("Folder not found", HttpStatusCode.NotFound);
                }
                return HttpMessage.ResponseMessage("", HttpStatusCode.NoContent);
            }
            catch (AuthenticationError e)
            {
                return HttpMessage.ResponseMessage(e.ErrorMessage, HttpStatusCode.Unauthorized);
            }           
        }
    }
}
