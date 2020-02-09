using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class CompetitionsController : ApiController
    {
        // GET: api/Competitions
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Competitions/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Competitions
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Competitions/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Competitions/5
        public void Delete(int id)
        {
        }
    }
}
