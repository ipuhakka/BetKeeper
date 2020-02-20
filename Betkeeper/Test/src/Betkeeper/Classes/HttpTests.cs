using System;
using System.Net;
using System.Net.Http;
using Betkeeper.Classes;
using Betkeeper.Exceptions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Betkeeper.Test.Classes
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
        public void CreateResponse_CasingChangedToCamelCase()
        {
            var camelCaseData = new
            {
                TestVar1 = 1
            };

            var response = Http.CreateResponse(
                HttpStatusCode.OK,
                camelCaseData);

            var dataAsDynamic = Http.GetHttpContent(response);

            Assert.AreEqual(1, (int)dataAsDynamic.testVar1);
            Assert.IsNull(dataAsDynamic.TestVar1);
        }

        [Test]
        public void GetHttpContent_WorksWithInt()
        {
            var testJsonContent = "{ 'testInt': 1}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetHttpContent(request);

            Assert.AreEqual(1, (int)asDynamic.testInt);
        }

        [Test]
        public void GetHttpContent_WorksWithDouble()
        {
            var testJsonContent = "{ 'testDouble': 2.8}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetHttpContent(request);

            Assert.AreEqual(2.8, (double)asDynamic.testDouble);
        }

        [Test]
        public void GetHttpContent_WorksWithString()
        {
            var testJsonContent = "{ 'testString': 'string'}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetHttpContent(request);

            Assert.AreEqual("string", asDynamic.testString.ToString());
        }

        [Test]
        public void GetHttpContent_WorksWithDateTime()
        {
            var testJsonContent = "{ 'testDateTime': '2019-01-01 12:20:20'}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetHttpContent(request);

            Assert.AreEqual(
                new DateTime(2019, 1, 1, 12, 20, 20), 
                (DateTime)asDynamic.testDateTime);
        }

        [Test]
        public void GetHttpContent_WorksWithNullValues()
        {
            var testJsonContent = "{ 'testNull': null}";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJsonContent);

            var asDynamic = Http.GetHttpContent(request);

            Assert.IsTrue(asDynamic.testNull == null);
        }
    }
}
