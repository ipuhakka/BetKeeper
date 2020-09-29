using Betkeeper.Data;
using Betkeeper.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Betkeeper.Models
{
    public class Participator
    {
        [Key]
        public int ParticipatorId { get; set; }

        public CompetitionRole Role { get; set; }

        public int UserId { get; set; }

        public int Competition { get; set; }
    }

    public class ParticipatorRepository
    {
        private CompetitionRepository CompetitionHandler { get; set; }

        private readonly BetkeeperDataContext _context;

        public ParticipatorRepository()
        {
            CompetitionHandler = new CompetitionRepository();
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        public List<Participator> GetParticipators(
            int? userId = null,
            int? competitionId = null,
            CompetitionRole? role = null)
        {
            var query = _context.Participator.AsQueryable();

            if (userId != null)
            {
                query = query.Where(participator => participator.UserId == userId);
            }

            if (competitionId != null)
            {
                query = query.Where(participator => participator.Competition == competitionId);
            }

            if (role != null)
            {
                query = query.Where(participator => participator.Role == role);
            }

            return query.ToList();
        }

        public void AddParticipator(int userId, int competitionId, CompetitionRole role)
        {
            // TODO: Käyttäjän validointi kun taulu tuodaan entitymalliin
            var competition = CompetitionHandler.GetCompetition(competitionId: competitionId);

            if (competition == null)
            {
                throw new ArgumentException("Competition does not exist");
            }

            _context.Participator.Add(new Participator
            {
                UserId = userId,
                Competition = competitionId,
                Role = role
            });

            _context.SaveChanges();
        }
    }
}
