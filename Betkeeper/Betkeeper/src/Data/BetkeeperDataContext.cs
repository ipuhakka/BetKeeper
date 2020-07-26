﻿using Betkeeper.Enums;
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
                    scoring => JsonConvert.SerializeObject(scoring),
                    scoring => JsonConvert.DeserializeObject<List<Scoring>>(scoring));

            modelBuilder
                .Entity<Target>()
                .Property(target => target.Type)
                .HasConversion(
                    target => (int)target,
                    target => (TargetType)target);

            modelBuilder
                .Entity<Target>()
                .Property(target => target.Selections)
                .HasConversion(
                    selections => JsonConvert.SerializeObject(selections),
                    selections => JsonConvert.DeserializeObject<List<string>>(selections));

            modelBuilder
                .Entity<Participator>()
                .Property(participator => participator.Role)
                .HasConversion(
                    role => (int)role,
                    role => (CompetitionRole)role);
        }
    }
}
