using System;
using System.Runtime.Serialization;

namespace Betkeeper.Exceptions
{
    [Serializable]
    public class UsernameInUseException: Exception, ISerializable
    {

        public string ErrorMessage { get; }

        public UsernameInUseException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        protected UsernameInUseException(SerializationInfo info, StreamingContext context)
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
