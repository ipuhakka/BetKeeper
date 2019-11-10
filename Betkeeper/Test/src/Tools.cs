using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Http.Controllers;
using NUnit.Framework;
using Newtonsoft.Json;
using Betkeeper;

namespace TestTools
{
    public static class Tools
    {
        public static HttpControllerContext MockHttpControllerContext(
            object dataContent = null, 
            Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage();

            request.Content = new StringContent(JsonConvert.SerializeObject(dataContent));

            if (headers != null)
            {
                foreach (var headerDictRow in headers)
                {
                    request.Headers.Add(headerDictRow.Key, headerDictRow.Value);
                }
            }

            return new HttpControllerContext
            {
                Request = request
            };
        }
    }
}
