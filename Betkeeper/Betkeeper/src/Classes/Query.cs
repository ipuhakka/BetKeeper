using System.Collections.Generic;

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

        public void AddIntParameterList(
            string columnKey,
            List<int> intList)
        {
            if (intList == null
                || intList.Count == 0)
            {
                return;
            }

            var intParametersInQuery = new List<string>();

            QueryString += QueryString.Contains(" WHERE ")
                ? " AND "
                : " WHERE ";

            QueryString += $"{columnKey} IN (";

            intList.ForEach(intValue =>
            {
                intParametersInQuery.Add($"@intValue{intValue}");
                QueryParameters.Add($"intValue{intValue}", intValue);
            });

            QueryString += string.Join(", ", intParametersInQuery);

            QueryString += ")";
        }
    }
}
