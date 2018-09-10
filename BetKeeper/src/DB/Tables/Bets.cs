using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using BetKeeper.Exceptions;

namespace BetKeeper.DB.Tables
{
    /// <summary>
    /// handles database operations for table bets.
    /// </summary>
    public class Bets
    {
        /// <summary>
        /// Returns all rows from table bets which match the query.
        /// </summary>
        /// <param name="user">If inputted, only bets from user with given username are searched.</param>
        /// <param name="bet_won"> If not null, used to return bets which have finished or not.</param>
        /// <param name="ConnectionString">Connectionstring to be used.</param>
        public static List<Bet> GetBets(string ConnectionString, int user = -1, bool? bet_finished = null)
        {
            string query = "SELECT * FROM bets ";

            if (user != -1)
                query = query + "WHERE owner = @owner ";

            if (user != -1 && bet_finished != null)
                if (bet_finished == true)
                    query = query + String.Format("AND bet_won != -1");
                else if (bet_finished == false)
                    query = query + String.Format("AND bet_won == -1");
            else if (bet_finished != null)
            {
                if (bet_finished == true)
                    query = query + String.Format("WHERE bet_won != -1");
                else if (bet_finished == false)
                    query = query + String.Format("AND bet_won == -1");
            }

            query = query + ";";
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("owner", user);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Bet> result = new List<Bet>();

            while (reader.Read())
            {
                Bet obj = new Bet(Convert.ToInt32(reader["owner"].ToString()), reader["name"].ToString(), reader["date_time"].ToString(), Convert.ToInt32(reader["bet_won"].ToString()),
                    Convert.ToInt32(reader["bet_id"].ToString()), Convert.ToDouble(reader["odd"].ToString()),
                    Convert.ToDouble(reader["bet"].ToString()));

                result.Add(obj);
            }
            con.Close();

            return result;
        }

        /// <summary>
        /// Returns a bet object.
        /// Returns null if no bets matched or query returned more than one bet.
        /// </summary>
        public static Bet GetBet(string ConnectionString, int id)
        {
            string query = "SELECT * FROM bets WHERE bet_id = @bet_id;";
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("bet_id", id);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Bet> bets = new List<Bet>();

            while (reader.Read())
            {
                Bet obj = new Bet(Convert.ToInt32(reader["owner"].ToString()), reader["name"].ToString(), reader["date_time"].ToString(), Convert.ToInt32(reader["bet_won"].ToString()),
                    Convert.ToInt32(reader["bet_id"].ToString()), Convert.ToDouble(reader["odd"].ToString()),
                    Convert.ToDouble(reader["bet"].ToString()));

                bets.Add(obj);
            }
            con.Close();

            return (bets.Count == 1) ? bets[0] : null;
        }

        /// <summary>
        /// returns all bets in specific folder.
        /// </summary>
        /// <param name="folder">Name of the folder.</param>
        /// <param name="owner">Folder owner id.</param>
        /// <param name="finished">Optional parameter, if not null, statement returns bets from folder
        /// that are either finished or not.</param>
        /// <param name="connectionString">Connectionstring used to connect to database.</param>
        /// <returns>Bets from the folder that match the inputted parameters.</returns>
        public static List<Bet> GetBetsInBetFolder(string ConnectionString, string folder, int owner, bool? bet_finished = null)
        {
            string query = "";
            if (bet_finished == null)
                query = "SELECT * FROM bet_in_bet_folder bf INNER JOIN bets b ON b.bet_id = bf.bet_id WHERE bf.owner = @owner and bf.folder = @folder;";
            else if (bet_finished == true)           
                query = "SELECT * FROM bet_in_bet_folder bf INNER JOIN bets b ON b.bet_id = bf.bet_id WHERE bf.owner = @owner and bf.folder = @folder and bet_won != -1;";
            else if (bet_finished == false)
                query = "SELECT * FROM bet_in_bet_folder bf INNER JOIN bets b ON b.bet_id = bf.bet_id WHERE bf.owner = @owner and bf.folder = @folder and bet_won = -1;";

            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();

            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("owner", owner);
            command.Parameters.AddWithValue("folder", folder);
            SQLiteDataReader reader = command.ExecuteReader();

            List<Bet> result = new List<Bet>();

            while (reader.Read())
            {
                Bet obj = new Bet(Convert.ToInt32(reader["owner"].ToString()), reader["name"].ToString(), reader["date_time"].ToString(), Convert.ToInt32(reader["bet_won"].ToString()),
                    Convert.ToInt32(reader["bet_id"].ToString()), Convert.ToDouble(reader["odd"].ToString()),
                    Convert.ToDouble(reader["bet"].ToString()));

                result.Add(obj);
            }
            con.Close();

            return result;
        }

        /// <summary>
        /// Adds a new bet to the bets table.
        /// </summary>
        /// <returns>Id of the last inserted row.</returns>
        /// <exception cref="ArgumentException">Thrown when datetime parameter could not be converted into a datetime object.</exception>
        /// <exception cref="SQLiteException">Thrown if foreign key constraints fail</exception>
        public static int CreateBet(string ConnectionString, int user_id, string datetime, double odd, double bet, string name, bool? bet_won)
        {
            int result = -1;
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            DateTime date;

            if (bet_won != null)
                result = Convert.ToInt32(bet_won);

            if (DateTime.TryParse(datetime, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out date))
                datetime = date.ToString("yyyy-MM-dd H:mm:ss");
            else
                throw new ArgumentException("Date inputted is not valid");

            string query = "INSERT INTO bets (bet_won, name, odd, bet, date_time, owner) values (@bet_won, @name, @odd, @bet, @date_time, @owner);";

            int queryResult;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("bet_won", result);
            command.Parameters.AddWithValue("name", name);
            command.Parameters.AddWithValue("odd", odd);
            command.Parameters.AddWithValue("bet", bet);
            command.Parameters.AddWithValue("date_time", datetime);
            command.Parameters.AddWithValue("owner", user_id);
            try
            {
                command.ExecuteNonQuery();
                queryResult = (int)con.LastInsertRowId;   
            }
            finally
            {
                con.Close();
            }
            return queryResult;
        }

        /// <summary>
        /// Adds bet to specific folders. Ignores folders which do not belong to selected user.
        /// </summary>
        /// <returns>Number of inserted rows.</returns>
        /// <exception cref="UnknownBetError">Thrown when bet with bet_id 'id' is not found.</exception>
        /// <exception cref="SQLiteException">Thrown if foreign key constraints fail</exception>
        public static int AddBetToFolders(string ConnectionString, List<string> folders, int bet_id, int user_id)
        {
            if (GetBet(ConnectionString, bet_id) == null)
                throw new UnknownBetError("Bet with id " + bet_id + " was not found");

            string query = "";
            List<string> user_folders = Folders.GetUsersFolders(ConnectionString, user_id);
            foreach(string folder in folders)
            {
                if (user_folders.Contains(folder))
                    query = query + String.Format("INSERT INTO bet_in_bet_folder VALUES ('{0}', @owner, @bet_id);", folder);
            }

            int queryResult;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("owner", user_id);
            command.Parameters.AddWithValue("bet_id", bet_id);
            try
            {
                queryResult = command.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
            return queryResult;
        }

        /// <summary>
        /// Modifies a bet which matches bet_id. Can modify result, odd, bet, and name for the bet.
        /// Returns number of affected rows. 
        /// </summary>
        /// <exception cref="UnknownBetError">Thrown when bet with bet_id 'id' is not found.</exception>
        /// <exception cref="AuthenticationError">Thrown when user tries to modify another user's bet.</exception>
        public static int ModifyBet(string ConnectionString, int bet_id, int user_id, bool? bet_won, double? odd = null, double? bet = null, string name = null)
        {
            int result = -1;
            if (GetBet(ConnectionString, bet_id) == null)
                throw new UnknownBetError("Bet with id " + bet_id + " was not found");

            if (GetBet(ConnectionString, bet_id).getOwner() != user_id)
                throw new AuthenticationError("Bet did not belong to user trying to modify it");

            if (bet_won != null)
                result = Convert.ToInt32(bet_won);

            string query = "UPDATE bets SET bet_won = @bet_won";

            if (odd != null)
                query = query + ", odd = @odd";
            if (bet != null)
                query = query + ", bet = @bet";
            if (!String.IsNullOrEmpty(name))
                query = query + ", name = @name";

            query = query + String.Format(" WHERE bet_id = {0};", bet_id);

            int queryResult;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("bet_won", result);
            command.Parameters.AddWithValue("odd", odd);
            command.Parameters.AddWithValue("bet", bet);
            command.Parameters.AddWithValue("name", name);
            try
            {
                queryResult = command.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
            return queryResult;
        }

        /// <summary>
        /// Deletes the bet from table bets that matches bet_id. Returns number of rows affected by the statement.
        /// Deletes also all matches from table bet_in_bet_folder, since bet deletion cascades.
        /// </summary>
        /// <param name="bet_id">id of the bet to be deleted.</param>
        /// <param name="user_id">Id of the user deleting the bet.</param>
        /// <exception cref="UnknownBetError">Thrown when bet with bet_id 'id' is not found.</exception>
        /// <exception cref="AuthenticationError">Thrown when user tries to delete another user's bet.</exception>
        public static int DeleteBet(string ConnectionString, int bet_id, int user_id)
        {
            if (GetBet(ConnectionString, bet_id) == null)
                throw new UnknownBetError("Bet with id " + bet_id + " was not found"); 

            if (GetBet(ConnectionString, bet_id).getOwner() != user_id)
                throw new AuthenticationError("Bet did not belong to user trying to delete it");

            string query = "DELETE FROM bets WHERE bet_id = @bet_id";

            int queryResult;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("bet_id", bet_id);
            try
            {
                queryResult = command.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
            return queryResult;
        }

        /// <summary>
        /// Removes a selected bet from selected folders.
        /// </summary>
        /// <exception cref="UnknownBetError">Thrown when bet with bet_id 'bet_id' is not found.</exception>
        /// <exception cref="AuthenticationError">Thrown when user tries to delete another user's bet.</exception>
        public static int DeleteBetFromFolders(string ConnectionString, int bet_id, int user_id, List<string> folders)
        {
            Bet bet = GetBet(ConnectionString, bet_id);
            if (bet == null)
                throw new UnknownBetError("Bet with id " + bet_id + " was not found");

            if (bet.getOwner() != user_id)
                throw new AuthenticationError("Bet did not belong to user trying to delete it");

            if (folders == null || folders.Count == 0)
                return -1;

            string query = "DELETE FROM bet_in_bet_folder WHERE bet_id = @bet_id AND (";

            for (int i = 0; i < folders.Count; i++)
            {
                query = query + String.Format("folder = '{0}' ", folders[i]);

                if (i < folders.Count - 1)
                    query = query + "OR ";
            }
            query = query + ");";

            int queryResult;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("bet_id", bet_id);
            try
            {
                queryResult = command.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
            return queryResult;
        }
    }
}
