using System;
using System.Runtime.Serialization;

namespace Betkeeper.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception, ISerializable
    {
        public string ErrorMessage { get; }

        public NotFoundException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        protected NotFoundException(SerializationInfo info, StreamingContext context)
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
