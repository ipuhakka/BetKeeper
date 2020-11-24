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

        public DbSet<TargetBet> TargetBet { get; set; }

        public DbSet<Folder> Folder { get; set; }

        public DbSet<BetInBetFolder> BetInBetFolder { get; set; }

        public DbSet<Bet> Bet { get; set; }

        public DbSet<ErrorLog> ErrorLog { get; set; }

        public BetkeeperDataContext(DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder.Options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Target>()
                .Property(target => target.ScoringDeprecated)
                .HasConversion(
                    scoring => JsonConvert.SerializeObject(scoring),
                    scoring => JsonConvert.DeserializeObject<List<ScoringDeprecated>>(scoring));

            modelBuilder
                .Entity<Target>()
                .Property(target => target.Scoring)
                .HasConversion(
                    scoring => JsonConvert.SerializeObject(scoring),
                    scoring => JsonConvert.DeserializeObject<Scoring>(scoring));

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
                .Entity<Target>()
                .Property(target => target.Result)
                .HasConversion(
                    result => JsonConvert.SerializeObject(result),
                    result => JsonConvert.DeserializeObject<TargetResultItem>(result));

            modelBuilder
                .Entity<Participator>()
                .Property(participator => participator.Role)
                .HasConversion(
                    role => (int)role,
                    role => (CompetitionRole)role);

            modelBuilder
                .Entity<Folder>()
                .HasKey(folder => new { folder.FolderName, folder.Owner });

            modelBuilder
                .Entity<BetInBetFolder>()
                .HasKey(folderBet => new { folderBet.FolderName, folderBet.BetId });

            modelBuilder
                .Entity<Bet>()
                .Property(bet => bet.BetResult)
                .HasConversion(
                    betResult => (int)betResult,
                    betResult => (BetResult)betResult);
        }
    }
}
