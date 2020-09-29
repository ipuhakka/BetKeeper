using Microsoft.EntityFrameworkCore;

namespace Betkeeper
{
    public static class Settings
    {
        public static string DatabasePath { get; set; }

        public static bool? UseForeignKeys { get; set; }

        /// <summary>
        /// SQL connection string
        /// </summary>
        public static string ConnectionString { get; set; }

        public static DbContextOptionsBuilder OptionsBuilder { get; private set; }

        public static void InitializeOptionsBuilderService(DbContextOptionsBuilder optionsBuilder = null)
        {
            if (optionsBuilder == null)
            {
                // TODO: Korjaa ja poista kun kaikki entitymallissa
                var connectionString = Settings.ConnectionString.Replace("Data Source", "Server");

                OptionsBuilder = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString);
            }
            else
            {
                OptionsBuilder = optionsBuilder;
            }
        }
    }
}
