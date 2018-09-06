using System;

namespace BetKeeper.Exceptions
{
    public class AuthenticationError : Exception
    {
        public string ErrorMessage;

        public AuthenticationError(string message)
        {
            ErrorMessage = message;
        }
    }
}
