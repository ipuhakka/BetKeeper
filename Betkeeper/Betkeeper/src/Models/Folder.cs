using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Betkeeper.Models
{
    [Table("bet_folders")]
    public class Folder
    {
        [Column("folder_name")]
        public string FolderName { get; set; }

        [Column("owner")]
        public int Owner { get; set; }
    }

    
    [Table("bet_in_bet_folder")]
    public class BetInBetFolder
    {
        [Column("folder")]
        public string FolderName { get; set; }

        [Column("owner")]
        public int Owner { get; set; }

        [Column("bet_id")]
        public int BetId { get; set; }
    }

    /// <summary>
    /// Class for handling folder and folder-bet related database actions
    /// </summary>
    public class FolderRepository
    {
        private readonly BetkeeperDataContext _context;

        public FolderRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        public List<string> GetUsersFolders(
            int userId,
            int? betId = null)
        {
            if (betId != null)
            {
                return GetFoldersByBet(userId, (int)betId)
                    .Select(betFolder => betFolder.FolderName)
                    .ToList();
            }

            return GetFolders(userId)
                .Select(folderEntity => folderEntity.FolderName)
                .ToList();
        }

        /// <summary>
        /// Returns folders based on query.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="folder"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        public List<Folder> GetFolders(
            int userId, 
            string folder = null)
        {
            var query = _context.Folder
                .Where(folderEntity => folderEntity.Owner == userId);

            if (folder != null)
            {
                query = query.Where(folderEntity => folderEntity.FolderName == folder);
            }

            return query.ToList();
        }

        /// <summary>
        /// Return folders in which
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        public List<BetInBetFolder> GetFoldersByBet(int userId, int betId)
        {
            return _context
                .BetInBetFolder
                .Where(folder =>
                    folder.Owner == userId
                    && folder.BetId == betId)
                .ToList();
        }

        public void AddFolder(int userId, string folderName)
        {
            _context.Folder.Add(new Folder
            {
                Owner = userId,
                FolderName = folderName
            });

            _context.SaveChanges();
        }

        public void DeleteFolder(int userId, string folderName)
        {
            _context.Folder.RemoveRange(_context.Folder.Where(folder =>
                folder.Owner == userId
                && folder.FolderName == folderName));

            _context.SaveChanges();
        }

        /// <summary>
        /// Checks if a bet exists in folder.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="folderName"></param>
        /// <param name="betId"></param>
        /// <returns></returns>
        public bool FolderHasBet(int userId, string folderName, int betId)
        {
            return _context.BetInBetFolder.Any(folder =>
                folder.FolderName == folderName 
                && folder.BetId == betId
                && folder.Owner == userId);
        }
    }
}
