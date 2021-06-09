using Betkeeper.Classes;
using Betkeeper.Data;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Betkeeper.Models
{
    [Table("Session")]
    public class Session
    {
        public string Token { get; set; }

        [Key]
        public int UserId { get; set; }

        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// Generate a new session token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Session GenerateSession(Token token)
        {
            return new Session 
            {
                Token = token.TokenString,
                UserId = token.Owner,
                ExpirationTime = DateTime.UtcNow.AddHours(24)
            };
        }
    }

    public class SessionRepository
    {
        private readonly BetkeeperDataContext _context;

        public SessionRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        public void AddSession(Session session)
        {
            _context.Sessions.Add(session);
            _context.SaveChanges();
        }

        public void UpdateUserSession(Token token)
        {
            var session = GetSession(token.Owner);

            session.Token = token.TokenString;
            session.ExpirationTime = DateTime.UtcNow.AddHours(24);

            _context.Update(session);
            _context.SaveChanges();
        }

        public void DeleteSession(int userId, string token)
        {
            var session = _context.Sessions.SingleOrDefault(session => session.UserId == userId && session.Token == token);

            if (session != null)
            {
                _context.Sessions.Remove(session);
                _context.SaveChanges();
            }
        }

        public Session GetSession(int userId, string token = null)
        {
            var query = _context.Sessions.Where(session => session.UserId == userId);

            if (token != null)
            {
                query = query.Where(session => session.Token == token);
            }

            return query.SingleOrDefault();
        }

        public Session GetSessionByToken(string token)
        {
            return _context.Sessions.SingleOrDefault(session => session.Token == token);
        }
    }
}
