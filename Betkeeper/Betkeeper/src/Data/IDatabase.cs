using System.Collections.Generic;
using System.Data;

namespace Betkeeper.Data
{
    public interface IDatabase
    {

        /// <summary>
        /// Performs a query to database and returns results as a DataTable.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        DataTable ExecuteQuery(
            string query,
            Dictionary<string, object> parameters = null);

        /// <summary>
        /// Executes a query and returns boolean.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        bool ReadBoolean(
            string query,
            Dictionary<string, object> parameters);

        int ReadInt(
            string query,
            Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <returns></returns>
        int ExecuteCommand(
            string query,
            Dictionary<string, object> parameters,
            bool returnLastInsertedRowId = false);
    }
}
