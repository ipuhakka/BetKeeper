using Betkeeper.Data;
using Betkeeper.Exceptions;
using Betkeeper.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Betkeeper.Models
{
    /// <summary>
    /// Model for Competition-table.
    /// </summary>
    public class Competition
    {
        [Key]
        public int CompetitionId { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        private Enums.CompetitionState? _state;
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.CompetitionState State
        {
            get
            {
                if (_state != null)
                {
                    return (Enums.CompetitionState)_state;
                }

                if (StartTime > DateTime.UtcNow)
                {
                    _state = Enums.CompetitionState.Open;
                    return (Enums.CompetitionState)_state;
                }

                var targets = TargetService.GetCompetitionTargets(CompetitionId);

                if (targets.Count == 0)
                {
                    _state = Enums.CompetitionState.Ongoing;
                    return (Enums.CompetitionState)_state;
                }

                if (targets.Count(target => target.TargetResultSet()) < targets.Count)
                {
                    _state = Enums.CompetitionState.Ongoing;
                    return (Enums.CompetitionState)_state;
                }

                _state = Enums.CompetitionState.Finished;
                return (Enums.CompetitionState)_state;
            }
        }

        public string JoinCode { get; set; }

        public string Description { get; set; }
    }

    /// <summary>
    /// Class for accessing competition data.
    /// </summary>
    public class CompetitionRepository
    {
        private readonly BetkeeperDataContext _context;

        public CompetitionRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        public void AddCompetition(Competition competition)
        {
            if (!Validate(competition))
            {
                throw new ArgumentException("Competition not valid to be added");
            }

            if (GetCompetitions(name: competition.Name).Count != 0)
            {
                throw new NameInUseException("Name already in use");
            }

            if (GetCompetitions(joinCode: competition.JoinCode).Count != 0)
            {
                throw new ArgumentException("Join code already in use");
            }

            _context.Competition.Add(competition);
            _context.SaveChanges();
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

            _context.Competition.Remove(competition);
            _context.SaveChanges();            
        }

        /// <summary>
        /// Gets competitions from database.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <param name="joinCode"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<Competition> GetCompetitions(
            int? competitionId = null,
            string name = null,
            string joinCode = null)
        {    
            var query = _context.Competition.AsQueryable();

            if (competitionId != null)
            {
                query = query.Where(competition => competition.CompetitionId == competitionId);
            }

            if (name != null)
            {
                query = query.Where(competition => competition.Name == name);
            }

            if (joinCode != null)
            {
                query = query.Where(competition => competition.JoinCode == joinCode);
            }

            return query.ToList();           
        }

        /// <summary>
        /// Get competitions by competitions ids.
        /// </summary>
        /// <param name="competitionIds"></param>
        /// <returns></returns>
        public List<Competition> GetCompetitionsById(List<int> competitionIds)
        {
            return _context
                .Competition
                .Where(competition =>
                    competitionIds.Contains(competition.CompetitionId))
                .ToList();            
        }

        /// <summary>
        /// Gets competition from database.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        public Competition GetCompetition(int? competitionId = null, string name = null)
        {
            var query = _context.Competition.AsQueryable();

            if (competitionId != null)
            {
                query = query.Where(competition => competition.CompetitionId == competitionId);
            }

            if (name != null)
            {
                query = query.Where(competition => competition.Name == name);
            }

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Validates a competition
        /// </summary>
        /// <param name="competition"></param>
        /// <returns></returns>
        private static bool Validate(Competition competition)
        {
            return competition != null
                && Enum.IsDefined(typeof(Enums.CompetitionState), competition.State)
                && !string.IsNullOrEmpty(competition.Name)
                && !string.IsNullOrEmpty(competition.JoinCode);
        }
    }
}
