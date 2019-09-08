using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper
{
    public static class Settings
    {
        public static string DatabasePath { get; set; }

        public static string GetConnectionString()
        {
            if (DatabasePath == null)
            {
                throw new ConfigurationException("Database path not set");
            }

            return string.Format("Data Source = {0}; Version = 3; foreign keys = true;", DatabasePath);
        }
    }
}
