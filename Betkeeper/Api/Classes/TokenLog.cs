using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Exceptions;

namespace Api.Classes
{
    public class TokenLog
    {
        static readonly List<Token> TokensInUse = new List<Token>();

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

        public static bool ContainsToken(string tokenString)
        {
            return TokensInUse.Any(token =>
                token.TokenString == tokenString);
        }

        public static int GetTokenOwner(string tokenString)
        {
            var owner = TokensInUse
                .FirstOrDefault(token =>
                    token.TokenString == tokenString)
                ?.Owner;

            if (owner == null)
            {
                throw new NotFoundException("TokenString did not match any active user");
            }

            return (int)owner;
        }
    }
}