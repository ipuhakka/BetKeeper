using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Models;
using Betkeeper.Enums;
using Betkeeper.Exceptions;

namespace Betkeeper.Actions
{
    /// <summary>
    /// Actions for competition bet targets.
    /// </summary>
    public class TargetAction
    {
        protected CompetitionRepository CompetitionRepository { get; set; }

        protected ParticipatorRepository ParticipatorRepository { get; set; }

        protected TargetRepository TargetRepository { get; set; }

        public void AddTarget(int userId, int competitionId, Target target)
        {
            var competition = CompetitionRepository.GetCompetition(competitionId);

            if (competition == null)
            {
                throw new NotFoundException("Competition does not exist");
            }

            if (ParticipatorRepository
                    .GetParticipators(userId, competitionId)
                    .Count == 0)
            {
                throw new InvalidOperationException("User not in competition");
            }

            if (competition.State > CompetitionState.Open)
            {
                throw new InvalidOperationException("Competition not open for new targets");
            }

            if (!ValidScoringForType(target))
            {
                throw new ArgumentException("Invalid scoring type for target");
            }

            if (target.Scoring
                .GroupBy(scoring => scoring.Score)
                .Any(group => group.Count() > 1))
            {
                throw new ArgumentException("Invalid scoring arguments");
            }

            TargetRepository.AddTarget(target);
        }

        private bool ValidScoringForType(Target target)
        {
            switch (target.Type)
            {
                case TargetType.OpenQuestion:
                case TargetType.Selection:
                    return target.Scoring.Count == 1
                        && target.Scoring[0].Score == TargetScore.CorrectResult;

                default:
                    return true;
            }
        }
    }
}
