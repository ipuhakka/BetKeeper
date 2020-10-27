using Betkeeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Betkeeper.Actions
{
    public class FolderAction
    {
        private FolderRepository FolderRepository { get; set; }

        public FolderAction()
        {
            FolderRepository = new FolderRepository();
        }

        public bool UserHasFolder(int userId, string folder)
        {
            return FolderRepository.GetFolders(userId, folder).Count > 0;
        }

        public void AddFolder(int userId, string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                throw new ActionException(
                    ActionExceptionType.InvalidInput,
                    $"Folder name cannot be empty");
            }

            if (UserHasFolder(userId, folder))
            {
                throw new ActionException(
                    ActionExceptionType.Conflict,
                    $"User already has folder named {folder}");
            }

            FolderRepository.AddFolder(userId, folder);
        }

        public void DeleteFolder(int userId, string folder)
        {
            if (!UserHasFolder(userId, folder))
            {
                throw new ActionException(
                    ActionExceptionType.NotFound,
                    $"Folder {folder} does not exist, could not delete");
            }

            FolderRepository.DeleteFolder(userId, folder);
        }

        /// <summary>
        /// Returns users folders
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betId">If given, folders in which bet is are returned</param>
        /// <returns></returns>
        public List<string> GetUsersFolders(int userId, int? betId = null)
        {
            return FolderRepository.GetUsersFolders(userId, betId: betId);
        }

        /// <summary>
        /// Checks if bet exists in folder.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="folder"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        public bool FolderHasBet(int userId, string folder, int betId)
        {
            return FolderRepository.FolderHasBet(userId, folder, betId);
        }

        /// <summary>
        /// Deletes a bet from specified folders
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betId"></param>
        /// <param name="folders"></param>
        public void DeleteBetFromFolders(int userId, int betId, List<string> folders)
        {
            FolderRepository.DeleteBetFromFolders(userId, betId, folders);
        }

        /// <summary>
        /// Add bet to folders
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betId"></param>
        /// <param name="folders"></param>
        public void AddBetToFolders(int userId, int betId, List<string> folders)
        {
            var betFolders = FolderRepository.GetFoldersByBet(userId, betId);
            var usersFolders = FolderRepository.GetFolders(userId);

            // If bet is already in some of the folders or user does not have all folders -> don't add
            if (folders.Any(folderName =>
                betFolders.Any(betFolder => betFolder.FolderName == folderName)
                || !usersFolders.Any(usersFolder => usersFolder.FolderName == folderName)))
            {
                throw new ActionException(
                    ActionExceptionType.Conflict,
                    "Cannot add bet to all specified folders");
            }

            FolderRepository.AddBetToFolders(userId, betId, folders);
        }
    }
}
