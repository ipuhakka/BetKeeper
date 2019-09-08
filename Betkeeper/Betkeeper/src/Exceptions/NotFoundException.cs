using System;

namespace Betkeeper.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        string ErrorMessage { get; }

        public NotFoundException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
