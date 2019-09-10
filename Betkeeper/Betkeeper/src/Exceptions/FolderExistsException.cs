using System;

namespace Betkeeper.Exceptions
{
    public class FolderExistsException : Exception
    {
        string ErrorMessage { get; }

        public FolderExistsException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
