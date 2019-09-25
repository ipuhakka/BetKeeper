using System;
using System.Runtime.Serialization;

namespace Betkeeper.Exceptions
{
    public class FolderExistsException : Exception, ISerializable
    {
        string ErrorMessage { get; }

        public FolderExistsException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        protected FolderExistsException(SerializationInfo info, StreamingContext context)
        {
            ErrorMessage = info.GetString("ErrorMessage");
        }

        void ISerializable.GetObjectData(SerializationInfo info,
           StreamingContext context)
        {
            info.AddValue("ErrorMessage", ErrorMessage);
        }
    }
}
