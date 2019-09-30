using System.Collections.Generic;

namespace Betkeeper.Models
{
    public interface IFolderModel
    {
        List<string> GetUsersFolders(int userId, int? betId = null);

        bool UserHasFolder(int userId, string folderName);

        bool FolderHasBet(int userId, string folderName, int betId);

        int AddNewFolder(int userId, string folderName);

        int DeleteFolder(int userId, string folderName);
    }
}
