using System;
using System.Data.SQLite;

namespace BetKeeper.DB.Tables
{
    public class Users
    {

        /// <summary>
        /// Adds a new user to users table.
        /// </summary>
        /// <param name="connectionString">connectionstring used.</param>
        /// <param name="username">Unique name for user.</param>
        /// <param name="password">Password for user.</param>
        /// <returns>-1 if username is in use, 1 if insertion was succesful.</returns>
        public static int AddUser(string connectionString, string username, string password)
        {
            if (UsernameExists(connectionString, username))
                return -1;

            string statement = String.Format("INSERT INTO users (username, password) VALUES ('{0}', '{1}');", username, password);
            queryUsers(statement, connectionString);
            return 1;
        }

        /// <summary>
        /// Return id of the user with given username.
        /// </summary>
        public static int GetUserId(string connectionString, string username)
        {
            string query = String.Format("SELECT user_id FROM users WHERE username = '{0}';", username);
            return queryInt(query, connectionString);
        }

        /// <summary>
        /// Returns true if username is already in use.
        /// </summary>
        private static bool UsernameExists(string connectionString, string username)
        {
            string query = String.Format("SELECT(EXISTS(SELECT 1 FROM users WHERE username = '{0}'));", username);
            return queryBoolean(query, connectionString);
        }

        /// <summary>
        /// Returns true if password for the user is correct
        /// </summary>
        public static bool PasswordIsCorrect(string connectionString, int user_id, string password)
        {
            string query = String.Format("SELECT(EXISTS(SELECT 1 FROM users WHERE user_id = {0} AND password='{1}'));", user_id, password);
            return queryBoolean(query, connectionString);
        }
        
        /// <summary>
        /// Makes a query to the database which returns either 0 or 1.
        /// </summary>
        private static bool queryBoolean(string query, string connectionString)
        {
            bool value = false;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                value = reader.GetBoolean(0);
            }

            con.Close();
            return value;
        }

        private static int queryInt(string query, string connectionString)
        {
            int value = -1;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                value = reader.GetInt32(0);
            }

            con.Close();
            return value;
        }

        /// <summary>
        /// Interface to handle sqlite-statements for table users.
        /// </summary>
        /// <exception cref="SQLiteException"></exception>
        public static void queryUsers(string query, string connectionString)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();

            con.Close();
        }
    }
}
