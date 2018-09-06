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
                query = String.Format("SELECT DISTINCT folder_name FROM bet_folders WHERE owner = {0}; ", user);
            else
                query = String.Format("select distinct folder from  bet_in_bet_folder bf WHERE bf.owner = '{0}' AND bet_id={1}; ", user, bet_id);
           
            return QueryFolders(query, connectionString);
        }

        /// <summary>
        /// Adds a new folder for the user. If user already has a folder with same name, folder is not created.
        /// </summary>
        /// <returns>-1 if folder wasn't created, 1 if creation was successful.</returns>
        public static int AddNewFolder(string connectionString, int user_id, string folder_name)
        {

            if (FolderExists(connectionString, user_id, folder_name))
                return -1;

            string statement = String.Format("INSERT INTO bet_folders VALUES ('{0}', {1});", folder_name, user_id);
            AddToFolders(statement, connectionString);
            return 1;
        }

        /// <summary>
        /// Deletes a folder from bet_folders. Returns the number of rows deleted.
        /// </summary>
        public static int DeleteFolder(string connectionString, int user_id, string folder_name)
        {
            string query = String.Format("DELETE FROM bet_folders WHERE owner = {0} and folder_name = '{1}';", user_id, folder_name);
            return QueryInt(query, connectionString);
        }

        /// <summary>
        /// returns true if a folder with given name exists and it belongs to the user with given id.
        /// </summary>
        private static bool FolderExists(string connectionString, int user_id, string folder)
        {
            string query = String.Format("SELECT(EXISTS(SELECT 1 FROM bet_folders WHERE owner = {0} AND folder_name = '{1}'));", user_id, folder);
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

        /// <summary>
        /// Executes an sqlite command which returns an integer value. 
        /// </summary>
        private static int QueryInt(string query, string connectionString)
        {
            int result = 0;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
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

        private static List<string> QueryFolders(string query, string connectionString)
        {
            List<string> results = new List<string>();
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                results.Add(reader.GetString(0));
            }
            con.Close();
            return results;
        }

        private static void AddToFolders(string query, string connectionString)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();
            con.Close();
        }
    }
}
