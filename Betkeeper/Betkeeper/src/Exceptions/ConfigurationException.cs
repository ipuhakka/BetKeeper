using System;

namespace Betkeeper.Exceptions
{
    [Serializable]
    public class ConfigurationException : Exception
    {
        public string Error { get; }

        public ConfigurationException(string error)
        {
            Error = error;
        }
    }
}
