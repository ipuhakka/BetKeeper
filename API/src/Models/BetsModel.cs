using System.Collections.Generic;
using Newtonsoft.Json;
using BetKeeper.Exceptions;
using BetKeeper.DB;
using System.IO;

namespace API.Models
{
    public class BetsModel
    {
        public string GetBets(string token, bool? finished, string folder)
        {
            int owner = TokenLog.GetTokenOwner(token);

            if (owner == -1)
            {
                throw new AuthenticationError("No owner for specified token was found");
            }

            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");

            if (folder == null)
                return JsonConvert.SerializeObject(db.GetBets(owner, finished));
            else
                return JsonConvert.SerializeObject(db.GetBetsInBetFolder(folder, owner, finished));
        }

        public int DeleteBet(string token, int id, List<string> folders)
        {
            int owner = TokenLog.GetTokenOwner(token);

            if (owner == -1)
            {
                throw new AuthenticationError("No owner for specified token was found");
            }

            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");

            if (folders == null || folders.Count == 0)
                return db.DeleteBet(id, owner);
            else
                return db.DeleteBetFromFolders(id, owner, folders);
        }

        public int ModifyBet(string token, int id, bool bet_won, double? odd, double? bet, string name)
        {
            int owner = TokenLog.GetTokenOwner(token);

            if (owner == -1)
            {
                throw new AuthenticationError("No owner for specified token was found");
            }

            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");

            return db.ModifyBet(id, owner, bet_won, odd, bet, name);
        }

        public int CreateBet(string token, string datetime, bool? bet_won, double odd, double bet, string name, List<string> folders)
        {
            int owner = TokenLog.GetTokenOwner(token);

            if (owner == -1)
            {
                throw new AuthenticationError("No owner for specified token was found");
            }

            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");

            return db.CreateBet(owner, datetime, bet_won, odd, bet, name, folders);
        }
    }
}
