using Betkeeper.Enums;
using Betkeeper.Exceptions;
using Betkeeper.Models;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Updates targets. Needs host rights
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="competitionId"></param>
        /// <param name="targets"></param>
        public void HandleTargetsUpdate(int userId, int competitionId, List<Target> targets)
        {
            var competition = CompetitionRepository.GetCompetition(competitionId);

            if (competition == null)
            {
                throw new NotFoundException("Competition does not exist");
            }

            if (ParticipatorRepository
                    .GetParticipators(userId, competitionId, CompetitionRole.Host)
                    .Count == 0)
            {
                throw new InvalidOperationException("User not in competition");
            }

            if (competition.State > CompetitionState.Open)
            {
                throw new InvalidOperationException("Competition not open for new targets");
            }

            ValidateTargets(targets);

            var updateList = targets
                .Where(target => target.TargetId != 0)
                .ToList();

            var insertList = targets
                .Where(target => target.TargetId == 0)
                .ToList();

            TargetRepository.UpdateTargets(updateList);
            TargetRepository.AddTargets(insertList);
        }

        /// <summary>
        /// Sets results for targets.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <param name="targets"></param>
        public void SetTargetResults(int competitionId, int userId, List<Target> targets)
        {
            // TODO: Yksikkötestit
            var competition = CompetitionRepository.GetCompetition(competitionId);

            if (competition == null)
            {
                throw new NotFoundException("Competition does not exist");
            }

            if (ParticipatorRepository
                    .GetParticipators(userId, competitionId, CompetitionRole.Host)
                    .Count == 0)
            {
                throw new InvalidOperationException("User not in competition");
            }

            if (competition.State == CompetitionState.Open)
            {
                throw new InvalidOperationException("Competition not started yet");
            }

            ValidateTargets(targets);

            var updateList = targets
                .Where(target => target.TargetId != 0)
                .ToList();

            var insertList = targets
                .Where(target => target.TargetId == 0)
                .ToList();

            TargetRepository.UpdateTargets(updateList);
            TargetRepository.AddTargets(insertList);
        }

        /// <summary>
        /// Returns targets for competition.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        public List<Target> GetTargets(int competitionId)
        {
            return TargetRepository.GetTargets(competitionId);
        }

        /// <summary>
        /// Removes a specified target.
        /// </summary>
        /// <param name="targetId"></param>
        public void RemoveTarget(int targetId)
        {
            TargetRepository.RemoveTarget(targetId);
        }

        /// <summary>
        /// Validates targets
        /// </summary>
        /// <param name="targets"></param>
        /// <exception cref="ActionException"></exception>
        private void ValidateTargets(List<Target> targets)
        {
            var i = 0;
            targets.ForEach(target =>
            {
                if (string.IsNullOrWhiteSpace(target.Bet))
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Row {i + 1}: No question given");
                }

                if (!target.HasScoringType(TargetScore.CorrectResult))
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Row {i + 1}: Missing points for correct result");
                }

                if (target.Type == TargetType.Result && !target.HasScoringType(TargetScore.CorrectWinner))
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Row {i + 1}: Missing points for correct winner");
                }

                if (!ValidScoringForType(target))
                {
                    throw new ActionException(
                        ActionExceptionType.ServerError,
                        $"Row {i + 1}: Invalid points");
                }

                if (target.Type == TargetType.Selection &&
                   (target.Selections == null || target.Selections.Count == 0))
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Row {i + 1}: No selections given for selection typed bet");
                }

                if (target.Type == TargetType.Result && 
                    !string.IsNullOrEmpty(target.Result?.Result))
                {
                    var scores = target.Result.Result.Split('-');

                    if (scores.Length != 2)
                    {
                        throw new ActionException(
                            ActionExceptionType.InvalidInput,
                            $"Target {i + 1} has invalid result");
                    }

                    if (!int.TryParse(scores[0], out int homescore)
                        || !int.TryParse(scores[1], out int awayscore))
                    {
                        throw new ActionException(
                            ActionExceptionType.InvalidInput,
                            $"Target {i + 1} has invalid result");
                    }
                }

                if (target.Scoring
                    .GroupBy(scoring => scoring.Score)
                    .Any(group => group.Count() > 1))
                {
                    throw new ArgumentException("Invalid scoring arguments");
                }

                i++;
            });
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
