using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Betkeeper.Data;

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

    public class UserRepository : BaseRepository
    {
        public List<string> GetUsernamesById(List<int> userIds)
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                return context
                    .User
                    .Where(user => userIds.Contains(user.UserId))
                    .Select(user => user.Username)
                    .ToList();
            }
        }

        public int? GetUserId(string username)
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                var userId = context
                    .User
                    .Where(user => user.Username == username)
                    .Select(user => user.UserId)
                    .FirstOrDefault();

                return userId == 0
                    ? null
                    : (int?)userId;
            }
        }

        public bool Authenticate(int userId, string password)
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                return context
                    .User
                    .Any(user => user.UserId == userId
                        && user.Password == password);
            }
        }
    }
}
