﻿using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Betkeeper.Actions
{
    public class CompetitionAction
    {
        private CompetitionRepository CompetitionRepository { get; set; }
        private ParticipatorRepository ParticipatorRepository { get; set; }

        public CompetitionAction()
        {
            CompetitionRepository = new CompetitionRepository();
            ParticipatorRepository = new ParticipatorRepository();
        }

        public Competition GetCompetition(int competitionId)
        {
            return CompetitionRepository.GetCompetition(competitionId: competitionId);
        }

        /// <summary>
        /// Creates a new competition.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="startTime"></param>
        public void CreateCompetition(
            int userId,
            string name,
            string description,
            DateTime startTime)
        {
            var joinCode = StringUtils.GenerateRandomString(6);

            while (CompetitionRepository.GetCompetitions(joinCode: joinCode).Count != 0)
            {
                joinCode = StringUtils.GenerateRandomString(6);
            }

            CompetitionRepository.AddCompetition(new Competition()
            {
                JoinCode = joinCode,
                Name = name,
                Description = description,
                StartTime = startTime
            });

            var competitionId = CompetitionRepository.GetCompetition(name: name)?.CompetitionId;

            if (competitionId == null)
            {
                throw new Exception($"Competition '{name}' not found, creation failed");
            }

            ParticipatorRepository
                .AddParticipator(
                    userId, (int)competitionId, Enums.CompetitionRole.Host);
        }

        public void DeleteCompetition(int userId, int competitionId)
        {
            var participator = ParticipatorRepository
                .GetParticipators(
                    userId: userId,
                    competitionId: competitionId,
                    role: Enums.CompetitionRole.Host)
                .FirstOrDefault();

            if (participator == null)
            {
                throw new InvalidOperationException();
            }

            CompetitionRepository.DeleteCompetition(competitionId);
        }

        /// <summary>
        /// Returns competitions where user is involved in.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Competition> GetUsersCompetitions(int userId)
        {
            var competitionIds = ParticipatorRepository
                .GetParticipators(userId: userId)
                .Select(participator => participator.Competition)
                .ToList();

            return CompetitionRepository
                .GetCompetitionsById(competitionIds)
                .OrderByDescending(competition => competition.StartTime)
                .ToList();
        }

        public void JoinCompetition(string joinCode, int userId)
        {
            var competition = CompetitionRepository
                .GetCompetitions(joinCode: joinCode)
                .FirstOrDefault();

            if (competition == null)
            {
                throw new ActionException(ActionResultType.NotFound, $"{joinCode} did not match any competition");
            }

            if (competition.State != Enums.CompetitionState.Open)
            {
                throw new ActionException(ActionResultType.Conflict, "Competition not open for new players");
            }

            if (ParticipatorRepository.GetParticipators(userId, competition.CompetitionId).Count > 0)
            {
                throw new ActionException(ActionResultType.Conflict, "Already joined competition");
            }

            ParticipatorRepository.AddParticipator(
                userId,
                competition.CompetitionId,
                Enums.CompetitionRole.Participator);
        }

        public Participator GetParticipator(int userId, int competitionId)
        {
            return ParticipatorRepository.GetParticipators(userId, competitionId).FirstOrDefault();
        }
    }
}
