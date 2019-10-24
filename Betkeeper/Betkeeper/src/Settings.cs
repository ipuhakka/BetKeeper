using Betkeeper.Exceptions;

namespace Betkeeper
{
    public static class Settings
    {
        public static string DatabasePath { get; set; }

        public static bool? UseForeignKeys { get; set; }

        public static string GetConnectionString()
        {
            // TODO: Asetuksista
            return "Data Source=betkeeper.database.windows.net;" +
                "Initial Catalog=betkeeper;" +
                "User id=betkeeper_admin;" +
                "Password=************;";
        }

        public static string GetSQLiteConnectionString()
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
