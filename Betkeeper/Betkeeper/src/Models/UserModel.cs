using System;
using System.Collections.Generic;
using Betkeeper.Data;
using Betkeeper.Exceptions;

namespace Betkeeper.Models
{
    public class UserModel : IUserModel
    {

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

            return Database.ExecuteCommand(
                commandText,
                new Dictionary<string, object>
                {
                    {"username", username },
                    {"password", password }
                });
        }

        /// <summary>
        /// Returns user id of matching username.
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <returns></returns>
        public int GetUserId(string username)
        {
            if (!UsernameInUse(username))
            {
                throw new NotFoundException(
                    string.Format("Username {0} not found", username));
            }

            var queryText = "SELECT user_id FROM users " +
                    "WHERE username=@username";

            return Database.ReadInt(
                queryText,
                new Dictionary<string, object>
                {
                    {"username", username }
                });
        }

        /// <summary>
        /// Checks if userId matches a given password.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(int userId, string password)
        {
            var query = "SELECT(EXISTS(SELECT 1 " +
                "FROM users " +
                "WHERE user_id = @user_id AND password=@password))";

            return Database.ReadBoolean(
                query,
                new Dictionary<string, object>
                {
                    { "user_id", userId },
                    { "password", password }
                });
        }

        public bool UsernameInUse(string username)
        {
            var query = "SELECT(EXISTS(SELECT 1 " +
                "FROM users " +
                "WHERE username = @username))";

            return Database.ReadBoolean(
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
            var query = "SELECT(EXISTS(SELECT 1 " +
                "FROM users " +
                "WHERE user_id = @userId))";

            return Database.ReadBoolean(
                query,
                new Dictionary<string, object>
                {
                    { "userId", userId }
                });
        }
    }
}
