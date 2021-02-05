using Betkeeper.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace Api.Classes
{
    public class TokenLog
    {
        static readonly List<Token> TokensInUse = new List<Token>();

        public static Token GetExistingToken(int userId)
        {
            return TokensInUse.FirstOrDefault(token =>
                token.Owner == userId);
        }

        public static Token CreateToken(int userId)
        {
            var token = new Token(userId);

            while (ContainsToken(token.TokenString))
            {
                token = new Token(userId);
            }

            TokensInUse.Add(token);

            return token;
        }

        /// <summary>
        /// Returns user id if request token is found.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int? GetUserIdFromRequest(HttpRequest request)
        {
            request.Headers.TryGetValue("Authorization", out var authorization);
            var tokenString = authorization.ToString();

            if (string.IsNullOrEmpty(tokenString)
                || !ContainsToken(tokenString))
            {
                return null;
            }

            return GetTokenOwner(tokenString);
        }

        /// <summary>
        /// Deletes token if it belogns to user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tokenString"></param>
        /// <exception cref="NotFoundException"></exception>
        public static void DeleteToken(int userId, string tokenString)
        {
            var tokenToRemove = TokensInUse.Find(token =>
                token.Owner == userId
                && token.TokenString == tokenString);

            if (tokenToRemove == null)
            {
                throw new NotFoundException(
                    string.Format("User with id {0} does not have specified tokenstring",
                    userId));
            }

            TokensInUse.Remove(tokenToRemove);
        }

        public static bool ContainsToken(string tokenString)
        {
            return TokensInUse.Any(token =>
                token.TokenString == tokenString);
        }

        /// <summary>
        /// Gets owner of tokenString.
        /// </summary>
        /// <param name="tokenString"></param>
        /// <returns></returns>
        public static int? GetTokenOwner(string tokenString)
        {
            return TokensInUse
                .FirstOrDefault(token =>
                    token.TokenString == tokenString)
                ?.Owner;
        }

        public static void ClearTokenLog()
        {
            TokensInUse.Clear();
        }
    }
}