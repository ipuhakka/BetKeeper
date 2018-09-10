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

            string statement = "INSERT INTO users (username, password) VALUES (@username, @password);";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(statement, con);
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("password", password);
            SQLiteDataReader reader = command.ExecuteReader();

            con.Close();
            return 1;
        }

        /// <summary>
        /// Return id of the user with given username.
        /// </summary>
        public static int GetUserId(string connectionString, string username)
        {
            string query = "SELECT user_id FROM users WHERE username = @username;";
            int value = -1;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("username", username);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                value = reader.GetInt32(0);
            }

            con.Close();
            return value;
        }

        /// <summary>
        /// Returns true if username is already in use.
        /// </summary>
        private static bool UsernameExists(string connectionString, string username)
        {
            string query = "SELECT(EXISTS(SELECT 1 FROM users WHERE username = @username));";
            bool value = false;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("username", username);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                value = reader.GetBoolean(0);
            }

            con.Close();
            return value;
        }

        /// <summary>
        /// Returns true if password for the user is correct, false if not.
        /// </summary>
        public static bool PasswordIsCorrect(string connectionString, int user_id, string password)
        {
            string query = "SELECT(EXISTS(SELECT 1 FROM users WHERE user_id = @user_id AND password=@password));";
            bool value = false;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("user_id", user_id);
            command.Parameters.AddWithValue("password", password);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                value = reader.GetBoolean(0);
            }

            con.Close();
            return value;
        }
    }
}
