using Betkeeper.Classes;
using Betkeeper.Models;
using Microsoft.AspNetCore.Http;
using System;

namespace Betkeeper.Actions
{
    public class SessionAction
    {
        /// <summary>
        /// Add session for user. If user already does not have a session, a new one is created.
        /// Otherwise old session is updated with a new token and expiration date.
        /// </summary>
        /// <param name="userId"></param>
        public static Token InstantiateSession(int userId)
        {
            var session = new SessionRepository().GetSession(userId);

            Token token;
            if (session == null)
            {
                token = new Token(userId);
                new SessionRepository().AddSession(Session.GenerateSession(token));
                return token;
            }

            if (session.ExpirationTime > DateTime.UtcNow)
            {
                return new Token(session);
            }

            // Session expired, create a new token for user and update session
            token = new Token(userId);
            new SessionRepository().UpdateUserSession(token);
            return token;       
        }

        /// <summary>
        /// Checks if user and token match, and session has not expired
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool SessionActive(int userId, string token)
        {
            var session = new SessionRepository().GetSession(userId, token);

            return session?.ExpirationTime > DateTime.UtcNow;
        }

        /// <summary>
        /// Returns user id, if session is active
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int? GetUserIdFromRequest(HttpRequest request)
        {
            request.Headers.TryGetValue("Authorization", out var authorization);
            var tokenString = authorization.ToString();

            if (string.IsNullOrEmpty(tokenString))
            {
                return null;
            }

            return new SessionRepository().GetSessionByToken(tokenString)?.UserId;
        }

        /// <summary>
        /// Delete a session
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        public static void DeleteSession(int userId, string token)
        {
            new SessionRepository().DeleteSession(userId, token);
        }
    }
}
