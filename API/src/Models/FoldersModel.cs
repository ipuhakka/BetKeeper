using System.IO;
using BetKeeper.DB;
using BetKeeper.Exceptions;
using Newtonsoft.Json;

namespace API.Models
{
    public class FoldersModel
    {

        public string GetUsersFolders(string token, int bet_id)
        {
            int owner = TokenLog.GetTokenOwner(token);

            if (owner == -1)
            {
                throw new AuthenticationError("No owner for specified token was found");
            }

            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");

            return JsonConvert.SerializeObject(db.GetUsersFolders(owner, bet_id));           
        }

        public int AddFolder(string token, string foldername)
        {
            int owner = TokenLog.GetTokenOwner(token);

            if (owner == -1)
            {
                throw new AuthenticationError("No owner for specified token was found");
            }

            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");

            return db.AddNewFolder(owner, foldername);
        }

        public int DeleteFolder(string token, string foldername)
        {
            int owner = TokenLog.GetTokenOwner(token);
            if (owner == -1)
            {
                throw new AuthenticationError("No owner for specified token was found");
            }

            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");

            return db.DeleteFolder(owner, foldername);
        }
    }
}
