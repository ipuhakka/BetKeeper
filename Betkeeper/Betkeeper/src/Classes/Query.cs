using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Classes
{
    /// <summary>
    /// Helper class for forming parameterized queries to database.
    /// </summary>
    public class Query
    {
        public string QueryString { get; set; }

        public Dictionary<string, object> QueryParameters { get; set; }

        public Query(string query, Dictionary<string, object> queryParameters)
        {
            QueryString = query;
            QueryParameters = queryParameters;
        }
    }
}
