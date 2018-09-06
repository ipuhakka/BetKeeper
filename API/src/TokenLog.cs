using System.Collections.Generic;
using API.Exceptions;

namespace API
{
    public class TokenLog
    {
        private static List<Token> tokens = new List<Token>(); 

        /// <summary>
        /// Returns true if tokens contain a string equal to tokenstring, false otherwise.
        /// </summary>
        public static bool ContainsToken(string tokenstring)
        {
            foreach (Token token in tokens)
            {
                if (tokenstring.Equals(token.GetToken()))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the id of token owner, given a token key.
        /// </summary>
        public static int GetTokenOwner(string token)
        {
            foreach (Token t in tokens)
            {
                if (t.GetToken().Equals(token))
                    return t.GetOwnerId();
            }

            return -1;
        }

        /// <summary>
        /// Adds a token to list if it is unique. Returns a new token if user did not have a token, otherwise returns 
        /// the already granted token.
        /// </summary>
        public static Token AddToken(Token t)
        {
            if (ContainsToken(t.GetToken()))
                throw new TokenInUse();

            foreach(Token token in tokens)
            {
                if (t.GetOwnerId().Equals(token.GetOwnerId()))
                    return token;
            }

            tokens.Add(t);
            return t;
        }
    }
}
