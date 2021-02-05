using System;
using System.Runtime.Serialization;

namespace Betkeeper.Exceptions
{
    [Serializable]
    public class ConfigurationException : Exception, ISerializable
    {
        public string ErrorMessage { get; }

        public ConfigurationException(string error)
        {
            ErrorMessage = error;
        }

        protected ConfigurationException(SerializationInfo info, StreamingContext context)
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
