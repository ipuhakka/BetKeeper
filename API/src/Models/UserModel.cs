using System.IO;
using BetKeeper.DB;

namespace API.Models
{
    public class UserModel
    {
        public int CreateUser(string username, string password)
        {
            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");
            return db.AddUser(username, password);
        }
    }
}
