using System;
using System.Collections.Generic;
using Betkeeper.Data;
using Betkeeper.Exceptions;

namespace Betkeeper.Repositories
{
    public class UserRepository : IUserRepository
    {
        public IDatabase _Database;

        public UserRepository(IDatabase database)
        {
            _Database = database;
        }

        public UserRepository()
        {
            _Database = new SQLDatabase();
        }

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <exception cref="UsernameInUseException"></exception>
        /// <returns>User id for created user.</returns>
        public int AddUser(string username, string password)
        {
            if (UsernameInUse(username))
            {
                throw new UsernameInUseException(
                    string.Format(
                        "Username {0} already in use, user not created", 
                        username));
            }

            var commandText = "INSERT INTO users(username, password) " +
                "VALUES(@username, @password)";

            return _Database.ExecuteCommand(
                commandText,
                new Dictionary<string, object>
                {
                    {"username", username },
                    {"password", password }
                });
        }

        public bool UsernameInUse(string username)
        {
            var query = "IF EXISTS (SELECT * FROM users WHERE username = @username)" +
                "BEGIN SELECT 1 END " +
                "ELSE BEGIN SELECT 0 END";

            return _Database.ReadBoolean(
                query,
                new Dictionary<string, object>
                {
                    { "username", username }
                });
        }

        /// <summary>
        /// Checks if userId is found.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UserIdExists(int userId)
        {
            var query = "IF EXISTS (SELECT * FROM users WHERE user_id = @userId)" +
                "BEGIN SELECT 1 END " +
                "ELSE BEGIN SELECT 0 END";

            return _Database.ReadBoolean(
                query,
                new Dictionary<string, object>
                {
                    { "userId", userId }
                });
        }
    }
}
