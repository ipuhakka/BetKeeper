using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Log a client error
        /// </summary>
        /// <param name="clientError"></param>
        /// <returns></returns>
        [Route("api/error")]
        public IActionResult Post(ClientError clientError)
        {
            clientError.LogError();

            return Ok();
        }
    }
}
