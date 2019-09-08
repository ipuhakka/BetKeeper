using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace Betkeeper.Data
{
    public static class Database
    {

        /// <summary>
        /// Performs a query to database and returns results as a DataTable.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(
            string query, 
            Dictionary<string, object> parameters)
        {
            using (var connection = new SQLiteConnection(Settings.GetConnectionString()))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand(query, connection);

                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                var dataTable = new DataTable();

                dataTable.Load(command.ExecuteReader());

                return dataTable;
            }
        }

        /// <summary>
        /// Executes a query and returns boolean.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool ReadBoolean(
            string query,
            Dictionary<string, object> parameters)
        {
            using (var connection = new SQLiteConnection(Settings.GetConnectionString()))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand(query, connection);

                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                var dataReader = command.ExecuteReader();

                var result = false;

                while (dataReader.Read())
                {
                    result = dataReader.GetBoolean(0);
                }

                return result;
            }
        }

        public static int ReadInt(
            string query,
            Dictionary<string, object> parameters)
        {
            using (var connection = new SQLiteConnection(Settings.GetConnectionString()))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand(query, connection);

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

                return result;
            }
        }

        /// <summary>
        /// Executes a command and returns number of rows affected.
        /// </summary>
        /// <returns></returns>
        public static int ExecuteCommand(
            string query,
            Dictionary<string, object> parameters)
        {
            using (var connection = new SQLiteConnection(Settings.GetConnectionString()))
            {
                connection.Open();

                SQLiteCommand command = new SQLiteCommand(query, connection);

                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                return command.ExecuteNonQuery();
            }
        }
    }
}
