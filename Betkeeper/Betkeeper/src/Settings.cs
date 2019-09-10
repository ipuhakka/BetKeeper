﻿using Betkeeper.Exceptions;

namespace Betkeeper
{
    public static class Settings
    {
        public static string DatabasePath { get; set; }

        public static bool? UseForeignKeys { get; set; }

        public static string GetConnectionString()
        {
            if (DatabasePath == null)
            {
                throw new ConfigurationException("Database path not set");
            }

            return string.Format("Data Source = {0}; Version = 3; foreign keys = {1};", 
                DatabasePath, UseForeignKeys ?? true);
        }
    }
}
