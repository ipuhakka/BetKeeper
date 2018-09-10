using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace BetKeeper.DB.Tables
{
    public class Folders
    {
        /// <summary>
        /// Gets a list of folder names for selected user. If bet_id is other than -1, returns folders in which the bet is in.
        /// </summary>
        public static List<string> GetUsersFolders(string connectionString, int user, int bet_id = -1)
        {
            string query = "";

            if (bet_id == -1)
                query = "SELECT DISTINCT folder_name FROM bet_folders WHERE owner = @owner; ";
            else
                query = "SELECT DISTINCT folder from  bet_in_bet_folder bf WHERE bf.owner = @owner AND bet_id = @bet_id; ";

            List<string> results = new List<string>();
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("owner", user);
            command.Parameters.AddWithValue("bet_id", bet_id);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                results.Add(reader.GetString(0));
            }
            con.Close();
            return results;
        }

        /// <summary>
        /// Adds a new folder for the user. If user already has a folder with same name, folder is not created.
        /// </summary>
        /// <returns>-1 if folder wasn't created, 1 if creation was successful.</returns>
        public static int AddNewFolder(string connectionString, int user_id, string folder_name)
        {

            if (FolderExists(connectionString, user_id, folder_name))
                return -1;

            string statement = "INSERT INTO bet_folders VALUES (@folder, @owner);";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(statement, con);
            command.Parameters.AddWithValue("folder", folder_name);
            command.Parameters.AddWithValue("owner", user_id);
            SQLiteDataReader reader = command.ExecuteReader();
            con.Close();
            return 1;
        }

        /// <summary>
        /// Deletes a folder from bet_folders. Returns the number of rows deleted.
        /// </summary>
        public static int DeleteFolder(string connectionString, int user_id, string folder_name)
        {
            string query = "DELETE FROM bet_folders WHERE owner = @owner AND folder_name = @folder;";
            int result = 0;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("owner", user_id);
            command.Parameters.AddWithValue("folder", folder_name);
            try
            {
                result = command.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
            return result;
        }

        /// <summary>
        /// returns true if a folder with given name exists and it belongs to the user with given id.
        /// </summary>
        private static bool FolderExists(string connectionString, int user_id, string folder)
        {
            string query = "SELECT(EXISTS(SELECT 1 FROM bet_folders WHERE owner = @owner AND folder_name = @folder));";
            bool value = false;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("owner", user_id);
            command.Parameters.AddWithValue("folder", folder);
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
