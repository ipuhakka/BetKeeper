﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Betkeeper;
using Betkeeper.Data;

namespace Betkeeper.Models
{
    public class Participator
    {
        [Key]
        public int ParticipatorId { get; set; }

        public int Role { get; set; }

        public int UserId { get; set; }

        public int Competition { get; set; }
    }

    public class ParticipatorRepository
    {
        protected DbContextOptionsBuilder OptionsBuilder { get; set; }

        protected CompetitionRepository CompetitionHandler { get; set; }

        public ParticipatorRepository()
        {
            // TODO: Tarkista onko tekstinmuokkaus tarpeellista
            var connectionString = Settings.ConnectionString.Replace("Data Source", "Server");

            OptionsBuilder = new DbContextOptionsBuilder()
                    .UseSqlServer(connectionString);

            CompetitionHandler = new CompetitionRepository();
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
                context.Participators.Add(new Participator
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