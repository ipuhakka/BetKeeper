using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Models
{
    public class User
    {
        public const string TableName = "Users";

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public void AddUser()
        {
            throw new NotImplementedException();
        }

        public static List<User> Authenticate(string username, string password)
        {
            throw new NotImplementedException();
        }

        public static bool UsernameInUse(string username)
        {
            throw new NotImplementedException();
        }
    }
}
