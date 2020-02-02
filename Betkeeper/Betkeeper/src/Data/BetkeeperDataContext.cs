using Microsoft.EntityFrameworkCore;
using Betkeeper.Models;

namespace Betkeeper.Data
{
    public class BetkeeperDataContext : DbContext
    {
        public DbSet<Competition> Competitions { get; set; }

        public DbSet<Participator> Participators { get; set; }

        public BetkeeperDataContext(DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder.Options)
        {
        }
    }
}
