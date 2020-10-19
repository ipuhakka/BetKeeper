using Microsoft.EntityFrameworkCore;

namespace Betkeeper
{
    public static class Settings
    {
        /// <summary>
        /// SQL connection string
        /// </summary>
        public static string ConnectionString { get; set; }

        public static string SecretKey { get; set; }

        public static DbContextOptionsBuilder OptionsBuilder { get; private set; }

        public static void InitializeOptionsBuilderService(DbContextOptionsBuilder optionsBuilder = null)
        {
            if (optionsBuilder == null)
            {
                // TODO: Korjaa ja poista kun kaikki entitymallissa
                var connectionString = ConnectionString.Replace("Data Source", "Server");

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
