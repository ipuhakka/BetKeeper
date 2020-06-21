using Betkeeper.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Betkeeper.Models
{

    [Table("users")]
    public class User
    {
        [Column("user_id")]
        [Key]
        public int UserId { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }
    }

    public class UserRepository : BaseRepository, IDisposable
    {
        private BetkeeperDataContext _context;

        public UserRepository()
        {
            _context = new BetkeeperDataContext(OptionsBuilder);
        }

        public UserRepository(BetkeeperDataContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public List<string> GetUsernamesById(List<int> userIds)
        {     
            return _context
                .User
                .Where(user => userIds.Contains(user.UserId))
                .Select(user => user.Username)
                .ToList();
        }

        public int? GetUserId(string username)
        {
            var userId = _context
                .User
                .Where(user => user.Username == username)
                .Select(user => user.UserId)
                .FirstOrDefault();

            return userId == 0
                ? null
                : (int?)userId;          
        }

        public bool Authenticate(int userId, string password)
        {
            return _context
                .User
                .Any(user => user.UserId == userId
                    && user.Password == password);           
        }
    }
}
