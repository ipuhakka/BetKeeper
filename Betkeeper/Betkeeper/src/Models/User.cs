using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
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

    public class UserRepository
    {
        protected DbContextOptionsBuilder OptionsBuilder { get; set; }

        public UserRepository()
        {
            // TODO: Poista kun kaikki entity mallissa
            var connectionString = Settings.ConnectionString.Replace("Data Source", "Server");

            OptionsBuilder = new DbContextOptionsBuilder()
                    .UseSqlServer(connectionString);
        }

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
    }
}
