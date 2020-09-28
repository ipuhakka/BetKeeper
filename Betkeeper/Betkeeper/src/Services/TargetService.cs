using Betkeeper.Data;
using Betkeeper.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Betkeeper.Services
{
    /// <summary>
    /// Service class for operations on targets when repositories cannot be used directly
    /// </summary>
    public static class TargetService
    {
        private static DbContextOptionsBuilder OptionsBuilder { get; set; }

        public static void InitializeTargetService(DbContextOptionsBuilder optionsBuilder = null)
        {
            OptionsBuilder = optionsBuilder ?? new DbContextOptionsBuilder()
                    .UseSqlServer(Settings.ConnectionString);
        }

        public static List<Target> GetCompetitionTargets(int competitionId)
        {
            return new TargetRepository(
                new BetkeeperDataContext(OptionsBuilder))
                .GetTargets(competitionId);
        }
    }
}
