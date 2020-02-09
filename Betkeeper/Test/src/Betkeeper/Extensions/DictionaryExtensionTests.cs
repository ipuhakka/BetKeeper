using System;
using System.Collections.Generic;
using Betkeeper.Extensions;
using NUnit.Framework;

namespace Betkeeper.Test.Extensions
{
    [TestFixture]
    public class DictionaryExtensionTests
    {
        [Test]
        public void GetString_NotInDictionary_ReturnsNull()
        {
            Assert.IsNull(new Dictionary<string, object>().GetString("key"));
        }

        [Test]
        public void GetString_InDictionary_ReturnsObjectAsString()
        {
            var dict = new Dictionary<string, object>
            {
                {"key1", 2 },
                {"key2", "2" }
            };

            Assert.AreEqual("2", dict.GetString("key1"));
            Assert.AreEqual("2", dict.GetString("key2"));
        }

        [Test]
        public void GetDateTime_NotInDictionary_ReturnsNull()
        {
            Assert.IsNull(new Dictionary<string, object>().GetDateTime("key"));
        }

        [Test]
        public void GetDateTime_NotValid_ReturnsNull()
        {
            var dict = new Dictionary<string, object>
            {
                {"key1", "2011-1-1 14:14:14gd" }
            };

            Assert.IsNull(dict.GetDateTime("key1"));
        }

        [Test]
        public void GetDateTime_Valid_ReturnsDateTime()
        {
            var dict = new Dictionary<string, object>
            {
                {"key1", "2011-01-02 14:14:14" },
                {"key2", new DateTime(2011, 1, 2, 14, 14, 14) }
            };

            Assert.AreEqual(new DateTime(2011, 1, 2, 14, 14, 14), dict.GetDateTime("key1"));
            Assert.AreEqual(new DateTime(2011, 1, 2, 14, 14, 14), dict.GetDateTime("key2"));
        }
    }
}
