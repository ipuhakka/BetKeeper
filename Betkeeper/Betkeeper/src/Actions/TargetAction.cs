using Betkeeper.Enums;
using Betkeeper.Exceptions;
using Betkeeper.Models;
using System;
using System.Linq;

namespace Betkeeper.Actions
{
    /// <summary>
    /// Actions for competition bet targets.
    /// </summary>
    public class TargetAction : IDisposable
    {
        private CompetitionRepository CompetitionRepository { get; set; }

        private ParticipatorRepository ParticipatorRepository { get; set; }

        private TargetRepository TargetRepository { get; set; }

        public TargetAction()
        {
            CompetitionRepository = new CompetitionRepository();
            ParticipatorRepository = new ParticipatorRepository();
            TargetRepository = new TargetRepository();
        }

        public TargetAction(
            CompetitionRepository competitionRepository,
            ParticipatorRepository participatorRepository,
            TargetRepository targetRepository)
        {
            CompetitionRepository = competitionRepository;
            ParticipatorRepository = participatorRepository;
            TargetRepository = targetRepository;
        }

        public void Dispose()
        {
            CompetitionRepository.Dispose();
            ParticipatorRepository.Dispose();
            TargetRepository.Dispose();
        }

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
