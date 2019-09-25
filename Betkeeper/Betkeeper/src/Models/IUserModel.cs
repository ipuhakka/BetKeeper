using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Models
{
    public interface IUserModel
    {
        bool Authenticate(int userId, string password);

        bool UsernameInUse(string username);

        bool UserIdExists(int userId);

        int GetUserId(string username);

        int AddUser(string username, string password);
    }
}
