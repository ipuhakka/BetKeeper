using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Models
{
    public interface IUserModel
    {
        /// <summary>
        /// Checks if userId matches a given password.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Authenticate(int userId, string password);

        bool UsernameInUse(string username);

        /// <summary>
        /// Checks if userId is found.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool UserIdExists(int userId);

        /// <summary>
        /// Returns user id of matching username.
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <returns></returns>
        int GetUserId(string username);

        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <exception cref="UsernameInUseException"></exception>
        /// <returns>User id for created user.</returns>
        int AddUser(string username, string password);
    }
}
