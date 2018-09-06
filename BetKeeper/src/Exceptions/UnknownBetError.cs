using System;

namespace BetKeeper.Exceptions
{
    public class UnknownBetError : Exception
    {
        public string ErrorMessage;

        public UnknownBetError(string message)
        {
            ErrorMessage = message;
        }
    }
}