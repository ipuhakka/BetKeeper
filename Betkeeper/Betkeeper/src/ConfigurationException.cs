using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper
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
