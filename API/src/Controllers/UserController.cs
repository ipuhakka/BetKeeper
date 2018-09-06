using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using API.Models;
using Newtonsoft.Json;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController: ApiController
    {
        /// <summary>
        ///  Adds a new row to table 'users'. 
        ///  
        /// Responses:
        ///     201 Created
        ///     400 Bad request
        ///     409 Conflict
        /// </summary>
        public HttpResponseMessage Post()
        {
            dynamic data;
            UserModel model = new UserModel();
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

            if (data["username"] == null || data["password"] == null)
            {
                return HttpMessage.ResponseMessage("Missing request body arguments", HttpStatusCode.BadRequest);
            }

            int res = model.CreateUser(data["username"].ToString(), data["password"].ToString());

            if (res == -1)
            {
                return HttpMessage.ResponseMessage("Creation failed: resource already exists", HttpStatusCode.Conflict);
            }
            else
            {
                return HttpMessage.ResponseMessage("Created user successfully", HttpStatusCode.Created);
            }
        }        

    }
}
