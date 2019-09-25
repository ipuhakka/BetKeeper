using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Betkeeper.Data;
using Betkeeper.Exceptions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Test.Betkeeper.Data
{
    [TestFixture]
    public class HttpTests
    {
        [Test]
        public void CreateResponse_ResponseSetCorrectly()
        {
            var httpResponse = Http.CreateResponse(
                HttpStatusCode.BadRequest,
                new NotFoundException("error"));

            HttpContent content = httpResponse.Content;
            string jsonContent = content.ReadAsStringAsync().Result;

            var notFoundException = JsonConvert.DeserializeObject<NotFoundException>(jsonContent);

            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
            Assert.AreEqual("error", notFoundException.ErrorMessage);
        }
    }
}
