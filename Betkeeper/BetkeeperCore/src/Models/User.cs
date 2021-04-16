using Betkeeper.Classes;
using Betkeeper.Data;
using Betkeeper.Exceptions;
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

    public class UserRepository
    {
        private readonly BetkeeperDataContext _context;

        public UserRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        /// <summary>
        /// Gets users by user id
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public List<User> GetUsersById(List<int> userIds)
        {
            return _context
                .User
                .Where(user => userIds.Contains(user.UserId))
                .ToList();
        }

        /// <summary>
        /// Returns users for existing usernames in list
        /// </summary>
        /// <param name="usernames"></param>
        /// <returns></returns>
        public List<User> GetUsersByName(List<string> usernames)
        {
            return _context.User
                .Where(user => usernames.Contains(user.Username))
                .ToList();
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
            var hashed = Security.Encrypt(password);

            return _context
                .User
                .Any(user => user.UserId == userId
                    && user.Password == hashed);           
        }

        public void ChangePassword(int userId, string newPassword)
        {
            var userEntity =_context.User.Single(user => user.UserId == userId);

            userEntity.Password = Security.Encrypt(newPassword);

            _context.Update(userEntity);
            _context.SaveChanges();
        }

        public bool UsernameInUse(string username)
        {
            return _context.User.Any(user => user.Username == username);
        }

        public void AddUser(string username, string password)
        {
            if (UsernameInUse(username))
            {
                throw new UsernameInUseException(
                    string.Format(
                        "Username {0} already in use, user not created",
                        username));
            }

            _context.User.Add(new User
            {
                Username = username,
                Password = Security.Encrypt(password)
            });

            _context.SaveChanges();
        }
    }
}
