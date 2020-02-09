using System;
using Betkeeper.Classes;
using Betkeeper.Models;

namespace Betkeeper.Actions
{
    public class CompetitionAction
    {
        protected CompetitionRepository CompetitionRepository { get; set; }
        protected ParticipatorRepository ParticipatorRepository { get; set; }

        public CompetitionAction()
        {
            CompetitionRepository = new CompetitionRepository();
            ParticipatorRepository = new ParticipatorRepository();
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
                State = (int)Enums.CompetitionState.Open,
                StartTime = startTime
            });

            var competitionId = CompetitionRepository.GetCompetition(name: name)?.CompetitionId;

            if (competitionId == null)
            {
                throw new Exception($"Competition '{name}' not found, creation failed");
            }

            ParticipatorRepository
                .AddParticipator(
                    userId, (int)competitionId, Enums.CompetitionRole.Admin);
        }
    }
}
