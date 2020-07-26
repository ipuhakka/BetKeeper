using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Betkeeper.Data
{
    public class SQLDatabase : IDatabase
    {
        /// <summary>
        /// Executes a command and returns either affectedRows 
        /// or the last inserted id.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="returnLastInsertedRowId"></param>
        /// <returns></returns>
        public int ExecuteCommand(string query, Dictionary<string, object> parameters, bool returnLastInsertedRowId = false)
        {
            using (var connection = new SqlConnection(
                Settings.ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    if (returnLastInsertedRowId)
                    {
                        command.CommandText += " SELECT SCOPE_IDENTITY();";

                        return int.Parse(
                            command.ExecuteScalar().ToString());
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }

        public DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SqlConnection(
               Settings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }

                var dataTable = new DataTable();

                dataTable.Load(command.ExecuteReader());

                command.Dispose();

                return dataTable;
            }
        }

        public bool ReadBoolean(string query, Dictionary<string, object> parameters)
        {
            using (var connection = new SqlConnection(
                Settings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                var dataReader = command.ExecuteReader();

                var result = false;

                while (dataReader.Read())
                {
                    result = Convert.ToBoolean(dataReader.GetInt32(0));
                }

                command.Dispose();

                return result;
            }
        }

        public int ReadInt(string query, Dictionary<string, object> parameters)
        {
            using (var connection = new SqlConnection(
                Settings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);

                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                var dataReader = command.ExecuteReader();

                var result = -1;

                while (dataReader.Read())
                {
                    result = dataReader.GetInt32(0);
                }

                command.Dispose();

                return result;
            }
        }
    }
}
