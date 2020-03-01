namespace Betkeeper.Repositories
{
    public interface IUserRepository
    {
        bool UsernameInUse(string username);

        /// <summary>
        /// Checks if userId is found.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool UserIdExists(int userId);

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
