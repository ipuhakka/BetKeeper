﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Betkeeper.Models;

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
        }
    }
}
