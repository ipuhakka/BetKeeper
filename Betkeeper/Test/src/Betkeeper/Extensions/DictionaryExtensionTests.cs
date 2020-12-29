using Betkeeper.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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

        [Test]
        public void GetInt_ValueNull_ReturnsNull()
        {
            var dicts = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>(),
                new Dictionary<string, object>
                {
                    { "Key", null}
                }
            };

            dicts.ForEach(dict =>
            {
                Assert.IsNull(dict.GetInt("Key"));
            });
        }

        [Test]
        public void GetInt_ValueNotInt_ReturnsNull()
        {
            var dict = new Dictionary<string, object>
            {
                { "Key1", 2.7 },
                { "Key2", "2.7" },
                { "Key3", true },
                { "Key4", "NotInt"}
            };

            foreach (KeyValuePair<string, object> kvp in dict)
            {
                Assert.IsNull(dict.GetInt(kvp.Key));
            }
        }

        [Test]
        public void GetInt_IsInt_ReturnsInt()
        {
            var dict = new Dictionary<string, object>
            {
                { "Key", 2}
            };

            Assert.AreEqual(2, dict.GetInt("Key"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("almost-but-no-cigar")]
        public void GetIdentifierFromKeyLike_NoMatch_ReturnsNull(string testCase)
        {
            var dict = new Dictionary<string, object>
            {
                { "almost-but-NO-CIGAR", 1 }
            };

            Assert.IsNull(dict.GetIdentifierFromKeyLike(testCase));
        }

        [Test]
        public void GetIdentifierFromKeyLike_CannotParseInt_ReturnsNull()
        {
            var dict = new Dictionary<string, object>
            {
                { "almost-but-no-number", 1 }
            };

            Assert.IsNull(dict.GetIdentifierFromKeyLike("almost-but-no"));
        }

        [Test]
        public void GetIdentifierFromKeyLike_Success_ReturnsInteger()
        {
            var dict = new Dictionary<string, object>
            {
                {"listgroup-bet-80", 1 }
            };

            Assert.AreEqual(80, dict.GetIdentifierFromKeyLike("listgroup"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("notexists")]
        public void GetDouble_NoValueFound_ReturnsNull(string testKey)
        {
            Assert.IsNull(new Dictionary<string, object>().GetDouble(testKey));
        }

        [TestCase("Not a double")]
        [TestCase(true)]
        public void GetDouble_NotADouble_ReturnsNull(object testCase)
        {
            var dict = new Dictionary<string, object>
            {
                { "test", testCase }
            };

            Assert.IsNull(dict.GetDouble("test"));
        }

        [TestCase("7", 7)]
        [TestCase("7,2", 7.2)]
        [TestCase("7.2", 7.2)]
        public void GetDouble_ValidDouble_ReturnsDouble(string doubleAsString, double expected)
        {
            var dict = new Dictionary<string, object>
            {
                { "test", doubleAsString }
            };

            Assert.AreEqual(expected, dict.GetDouble("test"));
        }

        [TestCase("list")]
        [TestCase("listGroup-")]
        [TestCase("listGroup-betListGroup-80")]
        public void GetKeyLike_Success_ReturnsKey(string testCase)
        {
            var dict = new Dictionary<string, object>
            {
                { "listGroup-betListGroup-80", 1 }
            };

            Assert.AreEqual("listGroup-betListGroup-80", dict.GetKeyLike(testCase));
        }

        [TestCase("listgroup")]
        [TestCase("listGroup-80")]
        [TestCase("listGroup-betListGroup-801")]
        public void GetKeyLike_NoMatchReturnsNull(string testCase)
        {
            var dict = new Dictionary<string, object>
            {
                { "listGroup-betListGroup-80", 1 }
            };

            Assert.IsNull(dict.GetKeyLike(testCase));
        }
    } 
}
