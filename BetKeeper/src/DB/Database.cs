using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using BetKeeper.DB.Tables;

namespace BetKeeper.DB
{
    public class Database
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// Creates a database file with specific name.
        /// </summary>
        /// <param name="filepath">Name and path of the file to be created.</param>
        public void CreateDatabase(string filepath)
        {
            SQLiteConnection.CreateFile(filepath);
        }

        /// <summary>Deletes a file given as the parameter.</summary>
        /// <param name="path">path to file to be deleted</param>
        /// <returns>-1 if file wasn't deleted, 1 on success.</returns>
        public int DeleteDatabase(string path)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (File.Exists(path))
                File.Delete(path);
            else
                return -1;

            return 1;
        }

        /// <summary>
        /// Creates tables defined in schemaFile.
        /// </summary>
        /// <param name="schemaFile">Path to an sql schema file.</param>
        /// <returns>1 on success, -1 if ConnectionString is not set or file is not of type '.sql.'</returns>
        public int CreateTables(string schemaFile)
        {
            return ExecuteScript(schemaFile);
        }

        /// <summary>
        /// Fills a database with data from a file.
        /// </summary>
        /// <param name="dataFile">Path to file that is used to fill the tables.</param>
        /// <returns>1 on success, -1 if ConnectionString is not set or file is not of type '.sql.'</returns>
        public int FillTables(string dataFile)
        {
            return ExecuteScript(dataFile);
        }

        /// <summary>
        /// Clears all tables from any data.
        /// </summary>
        public void ClearTables()
        {
            string command = "BEGIN; DELETE FROM bets; DELETE FROM bet_folders; DELETE FROM users; DELETE FROM bet_in_bet_folder; COMMIT;";
            Query(command);
        }

        /// <summary>
        /// Returns all rows from table bets which match the query.
        /// </summary>
        /// <param name="user">If inputted, only bets from user with given id are searched.</param>
        public List<Bet> GetBets(int user = -1, bool? finished = null)
        {
            return Bets.GetBets(ConnectionString, user, finished);
        }

        /// <summary>
        /// Returns a bet object.
        /// Returns null if no bets matched or query returned more than one bet.
        /// </summary>
        public Bet GetBet(int id)
        {
            return Bets.GetBet(ConnectionString, id);
        }

        /// <summary>
        /// returns all bets in specific folder.
        /// </summary>
        public List<Bet> GetBetsInBetFolder(string folder, int owner, bool? finished = null)
        {
            return Bets.GetBetsInBetFolder(ConnectionString, folder, owner, finished);
        }

        /// <summary>
        /// Modifies a bet which matches bet_id. Can modify result, odd, bet, and name for the bet.
        /// Returns number of affected rows. 
        /// </summary>
        public int ModifyBet(int bet_id, int user_id, bool? bet_won, double?  odd = null, double? bet = null, string name = null)
        {
            return Bets.ModifyBet(ConnectionString, bet_id, user_id, bet_won, odd, bet, name);
        }

        /// <summary>
        /// Deletes the bet from table bets that matches id. Returns number of rows affected by the statement.
        /// Deletes also all matches from table bet_in_bet_folder, since bet deletion cascades.
        /// </summary>
        public int DeleteBet(int id, int user_id)
        {
            return Bets.DeleteBet(ConnectionString, id, user_id);
        }

        /// <summary>
        /// Removes a selected bet from selected folders.
        /// </summary>
        public int DeleteBetFromFolders(int bet_id, int user_id, List<string> folders)
        {
            return Bets.DeleteBetFromFolders(ConnectionString, bet_id, user_id, folders);
        }

        /// <summary>
        /// Creates a new bet to the database. Returns id of the created bet row.
        /// </summary>
        /// <param name="bet">Stake for the bet.</param>
        /// <param name="datetime">Time the bet was logged to system.</param>
        /// <param name="finished">Indicates if the bet is resolved or still going.</param>
        /// <param name="folders">Folders which user has tagged the bet for.</param>
        /// <param name="name">Optional name for the bet for identification.</param>
        /// <param name="result">-1 if bet is not resolved, 0 if bet was lost, 1 if bet was won.</param>
        /// <param name="odd">Odd for the bet.</param>
        /// <param name="user">ID of the user who made the bet.</param>
        /// <exception cref="AuthenticationError"></exception>
        /// <exception cref="ArgumentException"></exception>
        public int CreateBet(int user_id, string datetime, bool? bet_won, double odd, double bet, string name = "", List<string> folders = null)
        {   /* add to bets, if folders is not null, get id of last insert and add to bet_in_bet_folder (foreach folder!)*/
            int id = Bets.CreateBet(ConnectionString, user_id, datetime, odd, bet, name, bet_won);

            if (folders != null && folders.Count > 0)
                Bets.AddBetToFolders(ConnectionString, folders, id, user_id);

            return id;
        }

        /// <summary>
        /// Gets a list of folder names for selected user. If bet_id is specified, returns folders in which the bet belongs to.
        /// </summary>
        public List<string> GetUsersFolders(int user_id, int bet_id = -1)
        {
            return Folders.GetUsersFolders(ConnectionString, user_id, bet_id);
        }

        /// <summary>
        /// Adds a new folder for the user. If user already has a folder with same name, folder is not created.
        /// </summary>
        /// <returns>-1 if folder wasn't created, 1 if creation was successful.</returns>
        public int AddNewFolder(int user_id, string folder_name)
        {
            return Folders.AddNewFolder(ConnectionString, user_id, folder_name);
        }

        /// <summary>
        /// Deletes a folder from bet_folders. Returns the number of rows deleted.
        /// </summary>
        public int DeleteFolder(int user_id, string folder_name)
        {
            return Folders.DeleteFolder(ConnectionString, user_id, folder_name);
        }

        /// <summary>
        /// Adds a new user to users table.
        /// </summary>
        /// <returns>-1 if username is in use, 1 if insertion was succesful.</returns>
        public int AddUser(string username, string password)
        {
            return Users.AddUser(ConnectionString, username, password);
        }

        /// <summary>
        /// Return id of the user with given username.
        /// </summary>
        public int GetUserId(string username)
        {
            return Users.GetUserId(ConnectionString, username);
        }

        /// <summary>
        /// Returns true if password for the user is correct
        /// </summary>
        public bool IsPasswordCorrect(int user_id, string password)
        {
            return Users.PasswordIsCorrect(ConnectionString, user_id, password);
        }

        /// <summary>
        /// Executes a script to database described in ConnectionString.
        /// </summary>
        /// <param name="path">Path to script file to use.</param>
        /// <returns>1 on success, -1 if ConnectionString is not set or file is not of type '.sql.'</returns>
        private int ExecuteScript(string path)
        {
            if (String.IsNullOrEmpty(ConnectionString) || Path.GetExtension(path) != ".sql")
                return -1;

            string sql = File.ReadAllText(path);
            Query(sql);

            return 1;
        }

        /// <summary>
        /// Performs an sqlite query to the database set with property ConnectionString.
        /// </summary>
        private void Query(string query)
        {
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.ExecuteNonQuery();
            con.Close();
        }

    }
}
