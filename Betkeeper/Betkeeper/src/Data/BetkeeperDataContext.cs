using Microsoft.EntityFrameworkCore;
using Betkeeper.Models;

namespace Betkeeper.Data
{
    public class BetkeeperDataContext : DbContext
    {
        public DbSet<Competition> Competition { get; set; }

        public DbSet<Participator> Participator { get; set; }

        public BetkeeperDataContext(DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder.Options)
        {
        }
    }
}
