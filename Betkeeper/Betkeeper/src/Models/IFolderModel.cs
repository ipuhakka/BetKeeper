using System.Collections.Generic;

namespace Betkeeper.Models
{
    public interface IFolderModel
    {
        /// <summary>
        /// Gets folders for specified user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        List<string> GetUsersFolders(int userId, int? betId = null);

        bool UserHasFolder(int userId, string folderName);

        bool FolderHasBet(int userId, string folderName, int betId);

        /// <summary>
        /// Adds a new folder to database.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="userId"></param>
        /// <exception cref="FolderExistsException"></exception>
        /// <returns>Number of rows affected.</returns>
        int AddNewFolder(int userId, string folderName);

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="folderName"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <returns>Number of rows affected.</returns>
        int DeleteFolder(int userId, string folderName);
    }
}
