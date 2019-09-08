using System;

namespace Betkeeper.Exceptions
{
    [Serializable]
    public class UsernameInUseException: Exception
    {

        public string ErrorMessage { get; }

        public UsernameInUseException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
