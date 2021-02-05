using System;
using System.Runtime.Serialization;

namespace Betkeeper.Exceptions
{
    [Serializable]
    public class NameInUseException : Exception, ISerializable
    {
        public string ErrorMessage { get; }

        public NameInUseException(string error)
        {
            ErrorMessage = error;
        }

        protected NameInUseException(SerializationInfo info, StreamingContext context)
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
