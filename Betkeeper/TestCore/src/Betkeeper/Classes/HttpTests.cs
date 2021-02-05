using Betkeeper.Classes;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTools;

namespace Betkeeper.Test.Classes
{

    [TestFixture]
    public class HttpTests
    {
        [Test]
        public void GetRequestBody_ParsesDictionaryContentCorrectly()
        {
            var controller = new TestController
            {
                ControllerContext = Tools.MockControllerContext(
                  dataContent: new Dictionary<string, object>
                  {
                      { "test1", "string" },
                      { "test2", 1 },
                      { "test3", true },
                      { "test4", null }
                  })
            };

            var resultDict = Http.GetRequestBody<Dictionary<string, object>>(controller.Request);
            Assert.AreEqual("string", resultDict["test1"]);
            Assert.AreEqual(1, resultDict["test2"]);
            Assert.AreEqual(true, resultDict["test3"]);
            Assert.IsTrue(resultDict.ContainsKey("test4"));
            Assert.IsNull(resultDict["test4"]);
        }

        [Test]
        public void GetRequestBody_ParsesClassContentCorrectly()
        {
            var controller = new TestController
            {
                ControllerContext = Tools.MockControllerContext(
                  dataContent: new TestClass 
                  {
                      Item = 2
                  })
            };

            var result = Http.GetRequestBody<TestClass>(controller.Request);

            Assert.AreEqual(2, result.Item);
        }

        public class TestController : ControllerBase
        {

        }

        public class TestClass
        {
            public int Item { get; set; }
        }
    }
}
