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

        [Test]
        public void GetRequestBody_WorksWithInt()
        {
            var testJsonContent = "{ 'testInt': 1}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetRequestBody(request);

            Assert.AreEqual(1, (int)asDynamic.testInt);
        }

        [Test]
        public void GetRequestBody_WorksWithDouble()
        {
            var testJsonContent = "{ 'testDouble': 2.8}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetRequestBody(request);

            Assert.AreEqual(2.8, (double)asDynamic.testDouble);
        }

        [Test]
        public void GetRequestBody_WorksWithString()
        {
            var testJsonContent = "{ 'testString': 'string'}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetRequestBody(request);

            Assert.AreEqual("string", asDynamic.testString.ToString());
        }

        [Test]
        public void GetRequestBody_WorksWithDateTime()
        {
            var testJsonContent = "{ 'testDateTime': '2019-01-01 12:20:20'}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetRequestBody(request);

            Assert.AreEqual(
                new DateTime(2019, 1, 1, 12, 20, 20), 
                (DateTime)asDynamic.testDateTime);
        }

        [Test]
        public void GetRequestBody_WorksWithNullValues()
        {
            var testJsonContent = "{ 'testNull': null}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetRequestBody(request);

            Assert.IsTrue(asDynamic.testNull == null);
        }
    }
}
