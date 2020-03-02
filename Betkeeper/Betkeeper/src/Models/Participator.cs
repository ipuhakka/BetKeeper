﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Betkeeper;
using Betkeeper.Data;
using System.Linq;

namespace Betkeeper.Models
{
    public class Participator
    {
        [Key]
        public int ParticipatorId { get; set; }

        // TODO: Enum ja conversio mallinrakennukseen
        public int Role { get; set; }

        public int UserId { get; set; }

        public int Competition { get; set; }
    }

    public class ParticipatorRepository : BaseRepository
    {
        protected CompetitionRepository CompetitionHandler { get; set; }

        public ParticipatorRepository()
        {
            CompetitionHandler = new CompetitionRepository();
        }

        public List<Participator> GetParticipators(
            int? userId = null, 
            int? competitionId = null,
            int? role = null)
        {
            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                var query = context.Participator.AsQueryable();

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
        }

        public void AddParticipator(int userId, int competitionId, Enums.CompetitionRole role)
        {
            // TODO: Käyttäjän validointi kun taulu tuodaan entitymalliin
            var competition = CompetitionHandler.GetCompetition(competitionId: competitionId);

            if (competition == null)
            {
                throw new ArgumentException("Competition does not exist");
            }

            using (var context = new BetkeeperDataContext(OptionsBuilder))
            {
                context.Participator.Add(new Participator
                {
                    UserId = userId,
                    Competition = competitionId,
                    Role = (int)role
                });

                context.SaveChanges();
            }
        }
    }
}
