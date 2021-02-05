using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Betkeeper.Extensions;
using NUnit.Framework;

namespace Betkeeper.Test.Extensions
{
    [TestFixture]
    public class JObjectExtensionTests
    {
        [Test]
        public void GetKeyLike_ReturnsNullWithNoMatch()
        {
            var jObject = new JObject
            {
                { "test", 2 }
            };

            Assert.IsNull(jObject.GetKeyLike("NotExists"));
        }

        [Test]
        public void GetKeyLike_ReturnsFirstMatch()
        {
            var jObject = new JObject
            {
                { "test", 2 },
                { "test2", 2 }
            };

            Assert.AreEqual("test", jObject.GetKeyLike("test"));
        }

        [Test]
        public void GetKeysLike_ReturnsAllMatches()
        {
            var jObject = new JObject
            {
                { "test-1", 2 },
                { "test-2", 2 },
                { "test", 2 }
            };

            var results = jObject.GetKeysLike("test-");

            Assert.AreEqual(2, results.Count);

            Assert.AreEqual("test-1", results[0]);
            Assert.AreEqual("test-2", results[1]);
        }
    }
}
