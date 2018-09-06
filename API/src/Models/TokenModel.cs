using System.IO;
using Newtonsoft.Json;
using BetKeeper.DB;
using API.Exceptions;

namespace API.Models
{
    public class TokenModel
    {
        public string username { get; set; }
        public string password { get; set; }

        /// <summary>
        /// Verify username and password, and return a random token of format
        /// 'xxxxxxxxxxxx', where x can be any digit from 0-9 or any uppercase or lowercase letter.
        /// If user verification is not successful, returns null.
        /// </summary>
        /// <returns></returns>
        public string GetToken(string username, string password)
        {
            Database db = new Database();
            db.ConnectionString = File.ReadAllText(@"app_files\connectionstring.txt");
            int user_id = db.GetUserId(username);
            bool uniqueToken = false;

            if (db.IsPasswordCorrect(user_id, password))
            {
                Token t = new Token(user_id);

                while (!uniqueToken)
                {
                    try
                    {
                        Token returnedToken = TokenLog.AddToken(t);
                        uniqueToken = true;
                        return JsonConvert.SerializeObject(returnedToken);
                    }
                    catch (TokenInUse)
                    {
                        uniqueToken = false;
                    }
                }
            }
            return null;
        } 
    }
}
