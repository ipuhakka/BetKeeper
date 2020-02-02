using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Betkeeper.Data;

namespace Betkeeper.Models
{
    /// <summary>
    /// Model for Competition-table.
    /// </summary>
    public class Competition
    {
        [Key]
        public int CompetitionId { get; set; }

        public string JoinCode { get; set; }

        public DateTime StartTime { get; set; }

        public int State { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// Class for accessing competition data.
    /// </summary>
    public class CompetitionRepository
    {
        protected DbContextOptionsBuilder OptionsBuilder { get; set; }

        public CompetitionRepository()
        {
            // TODO: Tarkista onko tekstinmuokkaus tarpeellista
            var connectionString = Settings.ConnectionString.Replace("Data Source", "Server");

            OptionsBuilder = new DbContextOptionsBuilder()
                    .UseSqlServer(connectionString);
        }

        /// <summary>
        /// Get competitions were user is involved in.
        /// </summary>
        /// <returns></returns>
        public List<Competition> GetUsersCompetitions()
        {
            throw new NotImplementedException();
        }

        public void AddCompetition(Competition competition)
        {
            if (!Validate(competition))
            {
                throw new ArgumentOutOfRangeException();
            }

            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                context.Competitions.Add(competition);
                context.SaveChanges();
            }
        }

        public void UpdateCompetition()
        {
            throw new NotImplementedException();
        }

        public void DeleteCompetition(int competitionId)
        {
            var competition = GetCompetition(competitionId);

            if (competition == null)
            {
                throw new InvalidOperationException("Competition not found");
            }

            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                context.Competitions.Remove(competition);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Validates a competition
        /// </summary>
        /// <param name="competition"></param>
        /// <returns></returns>
        private bool Validate(Competition competition)
        {
            return competition != null
                && Enum.IsDefined(typeof(Enums.CompetitionState), competition.State)
                && !string.IsNullOrEmpty(competition.Name)
                && !string.IsNullOrEmpty(competition.JoinCode);
        }

        /// <summary>
        /// Gets competitions from database.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        private List<Competition> GetCompetitions(int? competitionId = null)
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                var query = context.Competitions.AsQueryable();

                if (competitionId != null)
                {
                    query = query.Where(competition => competition.CompetitionId == competitionId);
                }

                return query.ToList();
            }
        }

        /// <summary>
        /// Gets competition from database.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        public Competition GetCompetition(int? competitionId = null)
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                var query = context.Competitions.AsQueryable();

                if (competitionId != null)
                {
                    query = query.Where(competition => competition.CompetitionId == competitionId);
                }

                return query.FirstOrDefault();
            }
        }
    }
}
