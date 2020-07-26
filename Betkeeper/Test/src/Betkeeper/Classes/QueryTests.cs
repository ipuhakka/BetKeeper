using Betkeeper.Classes;
using NUnit.Framework;
using System.Collections.Generic;

namespace Betkeeper.Test.Classes
{

    [TestFixture]
    public class QueryTests
    {

        [Test]
        public void Query_AddIntParameterList_EmptyList_DoesNotAddToQuery()
        {
            var query = new Query("SELECT * FROM bets WHERE owner=@userId",
                new Dictionary<string, object>
                {
                    { "userId", 1 }
                });

            query.AddIntParameterList("someKey", new List<int>());

            Assert.AreEqual("SELECT * FROM bets WHERE owner=@userId", query.QueryString);

            query.AddIntParameterList("someKey", null);

            Assert.AreEqual("SELECT * FROM bets WHERE owner=@userId", query.QueryString);
        }

        /// <summary>
        /// Forms a valid query when query contains previous where-conditions.
        /// </summary>
        [Test]
        public void Query_AddIntParameterList_FormsValidQueryWithOneParameter()
        {
            var query = new Query("SELECT * FROM bets WHERE owner=@userId",
                new Dictionary<string, object>
                {
                    { "userId", 1 }
                });

            query.AddIntParameterList("someKey", new List<int>
            {
                1
            });

            Assert.AreEqual(
                "SELECT * FROM bets WHERE owner=@userId AND " +
                "someKey IN (@intValue1)",
                query.QueryString);

            Assert.IsTrue(query.QueryParameters.Count == 2);
        }

        /// <summary>
        /// Forms a valid query when query does not contain previous where-conditions.
        /// </summary>
        [Test]
        public void Query_AddIntParameterList_FormsValidQueryWithOneParameterNoPreviousParameters()
        {
            var query = new Query("SELECT * FROM bets",
                new Dictionary<string, object>());

            query.AddIntParameterList("someKey", new List<int>
            {
                1
            });

            Assert.AreEqual(
                "SELECT * FROM bets WHERE " +
                "someKey IN (@intValue1)",
                query.QueryString);

            Assert.IsTrue(query.QueryParameters.Count == 1);
        }

        /// <summary>
        /// Forms a valid query when int list contains more than one parameter.
        /// </summary>
        [Test]
        public void Query_AddIntParameterList_FormsValidQueryWithThreeParametersNoPreviousParameters()
        {
            var query = new Query("SELECT * FROM bets",
                new Dictionary<string, object>());

            query.AddIntParameterList("someKey", new List<int>
            {
                1, 2, 3
            });

            Assert.AreEqual(
                "SELECT * FROM bets WHERE " +
                "someKey IN (@intValue1, @intValue2, @intValue3)",
                query.QueryString);

            Assert.IsTrue(query.QueryParameters.Count == 3);

            Assert.AreEqual((int)query.QueryParameters["intValue1"], 1);
            Assert.AreEqual((int)query.QueryParameters["intValue2"], 2);
            Assert.AreEqual((int)query.QueryParameters["intValue3"], 3);
        }
    }
}
