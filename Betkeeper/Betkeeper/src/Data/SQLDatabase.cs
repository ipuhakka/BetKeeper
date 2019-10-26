using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Betkeeper.Data
{
    public class SQLDatabase : IDatabase
    {
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

                    var affectedRows = command.ExecuteNonQuery();

                    if (returnLastInsertedRowId)
                    {
                        using (var command2 = new SqlCommand(
                        @"select last_insert_rowid()",
                        connection))
                        {
                            var lastInsertedId = (long)command2.ExecuteScalar();
                            return (int)lastInsertedId;
                        }
                    }

                    return affectedRows;
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
