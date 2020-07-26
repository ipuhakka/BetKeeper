using Microsoft.EntityFrameworkCore;


namespace Betkeeper.Models
{
    public class BaseRepository
    {
        protected DbContextOptionsBuilder OptionsBuilder { get; set; }

        public BaseRepository()
        {
            // TODO: Korjaa ja poista kun kaikki entitymallissa
            var connectionString = Settings.ConnectionString.Replace("Data Source", "Server");

            OptionsBuilder = new DbContextOptionsBuilder()
                    .UseSqlServer(connectionString);
        }
    }
}
