using Betkeeper.Classes;
using Betkeeper.Exceptions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;

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
        public void CreateResponse_DataNull_NotConvertedToString()
        {
            var httpResponse = Http.CreateResponse(HttpStatusCode.BadRequest);

            var content = httpResponse.Content;

            Assert.IsNull(content);
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

        [Test]
        public void GetHttpContent_WorksWithLowerCaseKeys()
        {
            var testJson = "{ 'testInt': 1, 'testString': 'tst', 'testDouble': 2.7, 'testBool': true }";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJson);

            var result = Http.GetHttpContent<ContentTest>(request.Content);

            Assert.AreEqual(1, result.TestInt);
            Assert.AreEqual("tst", result.TestString);
            Assert.AreEqual(2.7, result.TestDouble);
            Assert.IsTrue(result.TestBool);
        }

        [Test]
        public void GetHttpContent_WorksWithUpperCaseKeys()
        {
            var testJson = "{ 'TestInt': 1, 'TestString': 'tst', 'TestDouble': 2.7, 'TestBool': true }";

            var request = new HttpRequestMessage();
            request.Content = new StringContent(testJson);

            var result = Http.GetHttpContent<ContentTest>(request.Content);

            Assert.AreEqual(1, result.TestInt);
            Assert.AreEqual("tst", result.TestString);
            Assert.AreEqual(2.7, result.TestDouble);
            Assert.IsTrue(result.TestBool);
        }

        /// <summary>
        /// Class to test Getting typed Http content.
        /// </summary>
        private class ContentTest
        {
            public int TestInt { get; set; }

            public string TestString { get; set; }

            public double TestDouble { get; set; }

            public bool TestBool { get; set; }
        }
    }
}
