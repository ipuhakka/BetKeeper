using Betkeeper.Enums;
using Betkeeper.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Betkeeper.Data
{
    public class BetkeeperDataContext : DbContext
    {
        public DbSet<Competition> Competition { get; set; }

        public DbSet<Participator> Participator { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Target> Target { get; set; }

        public BetkeeperDataContext(DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder.Options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Target>()
                .Property(target => target.Scoring)
                .HasConversion(
                    target => target.ToString(),
                    target => JsonConvert.DeserializeObject<List<Scoring>>(target));

            modelBuilder
                .Entity<Target>()
                .Property(target => target.Type)
                .HasConversion(
                    target => (int)target,
                    target => (TargetType)target);

            modelBuilder
                .Entity<Participator>()
                .Property(participator => participator.Role)
                .HasConversion(
                    role => (int)role,
                    role => (CompetitionRole)role);
        }
    }
}
