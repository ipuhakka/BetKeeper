using Betkeeper.Data;
using Betkeeper.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Betkeeper.Actions
{
    public class TargetBetAction : IDisposable
    {
        private TargetRepository TargetRepository { get; set; }
        private TargetBetRepository TargetBetRepository { get; set; }
        private CompetitionRepository CompetitionRepository { get; set; }
        private ParticipatorRepository ParticipatorRepository { get; set; }

        public TargetBetAction()
        {
            TargetBetRepository = new TargetBetRepository();
            TargetRepository = new TargetRepository();
            CompetitionRepository = new CompetitionRepository();
            ParticipatorRepository = new ParticipatorRepository();
        }

        public TargetBetAction(BetkeeperDataContext context)
        {
            TargetBetRepository = new TargetBetRepository(context);
            TargetRepository = new TargetRepository(context);
            CompetitionRepository = new CompetitionRepository(context);
            ParticipatorRepository = new ParticipatorRepository(context);
        }

        public void Dispose()
        {
            TargetBetRepository.Dispose();
            TargetRepository.Dispose();
            CompetitionRepository.Dispose();
        }

        /// <summary>
        /// Adds targets.
        /// </summary>
        /// <param name="competitionId"></param>
        /// <param name="userId"></param>
        /// <param name="targetBets"></param>
        /// <exception cref="ActionException"></exception>
        public void AddTargetBets(
            int competitionId, 
            int userId,
            List<TargetBet> targetBets)
        {
            var competition = CompetitionRepository.GetCompetition(competitionId);

            if (competition == null 
                || competition.State != Enums.CompetitionState.Open)
            {
                throw new ActionException(
                    ActionExceptionType.Conflict, 
                    "Competition is not open for betting");
            }

            var competitionTargets = TargetRepository
                .GetTargets(competitionId: competitionId);

            // Is user in competition
            var participator = ParticipatorRepository.GetParticipators(
                userId: userId,
                competitionId: competitionId).SingleOrDefault();

            if (participator == null)
            {
                throw new ActionException(
                    ActionExceptionType.Unauthorized,
                    "User not in competition");
            }

            for (var i = 0; i < targetBets.Count; i++)
            {
                ValidateTarget(targetBets[i], competitionTargets, i);
            }

            TargetBetRepository.AddTargetBets(targetBets);
        }

        /// <summary>
        /// Validate bet target
        /// </summary>
        /// <param name="targetBet"></param>
        /// <param name="competitionTargets"></param>
        /// <param name="targetIndex"></param>
        private void ValidateTarget(
            TargetBet targetBet, 
            List<Target> competitionTargets,
            int targetIndex)
        {
            var target = competitionTargets.FirstOrDefault(
                competitionTarget => competitionTarget.TargetId == targetBet.Target);

            if (target == null)
            {
                throw new ActionException(
                    ActionExceptionType.Conflict,
                    "Target does not exist");
            }

            if (string.IsNullOrEmpty(targetBet.Bet))
            {
                throw new ActionException(
                    ActionExceptionType.InvalidInput,
                    $"Target {targetIndex + 1} is missing an answer");
            }

            if (target.Type == Enums.TargetType.Result)
            {
                var scores = targetBet.Bet.Split('-');

                if (scores.Length != 2)
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Target {targetIndex + 1} has invalid result");
                }

                if (!int.TryParse(scores[0], out int homescore) 
                    || !int.TryParse(scores[1], out int awayscore))
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Target {targetIndex + 1} has invalid result");
                }
            }

            else if (target.Type == Enums.TargetType.Selection)
            {
                if (!target.Selections.Contains(targetBet.Bet))
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Target {targetIndex + 1} has invalid selection");
                }
            }
        }
    }
}
