using System;
using System.Runtime.Serialization;

namespace Betkeeper.Exceptions
{
    [Serializable]
    public class ParsingException : Exception, ISerializable
    {
        public string ErrorMessage { get; }

        public ParsingException(string error)
        {
            ErrorMessage = error;
        }

        protected ParsingException(SerializationInfo info, StreamingContext context)
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
