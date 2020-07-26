using Betkeeper.Data;
using Betkeeper.Exceptions;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Betkeeper.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        public IDatabase _Database { get; }


        public FolderRepository()
        {
            _Database = new SQLDatabase();
        }

        public FolderRepository(IDatabase database)
        {
            _Database = database;
        }

        /// <summary>
        /// Gets folders for specified user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        public List<string> GetUsersFolders(int userId, int? betId = null)
        {
            var query = "";
            var parameters = new Dictionary<string, object>();

            if (betId == null)
            {
                query = "SELECT DISTINCT folder_name " +
                    "FROM bet_folders " +
                    "WHERE owner = @owner; ";

                parameters.Add("owner", userId);
            }
            else
            {
                query = "SELECT DISTINCT folder " +
                    "FROM  bet_in_bet_folder bf " +
                    "WHERE bf.owner = @owner AND bet_id = @betId; ";

                parameters.Add("owner", userId);
                parameters.Add("betId", betId);
            }

            var datatable = _Database.ExecuteQuery(
                query,
                parameters);

            var dataRows = datatable.Rows.Cast<DataRow>();

            return dataRows.Select(row =>
                row[0].ToString())
                .ToList();
        }

        public bool UserHasFolder(int userId, string folderName)
        {
            var query = "IF EXISTS (SELECT " +
                "* FROM bet_folders " +
                "WHERE owner = @userId AND folder_name = @folderName) " +
                "BEGIN SELECT 1 END " +
                "ELSE BEGIN SELECT 0 END";

            return _Database.ReadBoolean(
                query,
                new Dictionary<string, object>
                {
                    {"userId", userId },
                    {"folderName", folderName }
                });
        }

        public bool FolderHasBet(int userId, string folderName, int betId)
        {
            var query = "IF EXISTS (SELECT " +
                "* FROM bet_in_bet_folder " +
                "WHERE owner = @userId AND folder = @folderName " +
                "AND bet_id = @betId) " +
                "BEGIN SELECT 1 END " +
                "ELSE BEGIN SELECT 0 END";

            return _Database.ReadBoolean(
                query,
                new Dictionary<string, object>
                {
                    {"userId", userId },
                    {"folderName", folderName },
                    {"betId", betId }
                });
        }

        /// <summary>
        /// Adds a new folder database.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="userId"></param>
        /// <exception cref="FolderExistsException"></exception>
        /// <returns>Number of rows affected.</returns>
        public int AddNewFolder(int userId, string folderName)
        {
            if (UserHasFolder(userId, folderName))
            {
                throw new FolderExistsException(
                    string.Format(
                        "{0} already has folder named {1}",
                        userId,
                        folderName));
            }

            var query = "INSERT INTO bet_folders VALUES (@folder, @userId)";

            return _Database.ExecuteCommand(
                query,
                new Dictionary<string, object>
                {
                    {"folder", folderName },
                    {"userId", userId }
                });
        }

        /// <summary>
        /// Deletes a folder.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="folderName"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <returns>Number of rows affected.</returns>
        public int DeleteFolder(int userId, string folderName)
        {
            if (!UserHasFolder(userId, folderName))
            {
                throw new NotFoundException("Folder trying to be deleted not found");
            }

            var query = "DELETE FROM bet_folders WHERE owner = @userId AND folder_name = @folderName";

            return _Database.ExecuteCommand(
                query,
                new Dictionary<string, object>
                {
                    {"userId", userId },
                    {"folderName", folderName }
                });
        }
    }
}
