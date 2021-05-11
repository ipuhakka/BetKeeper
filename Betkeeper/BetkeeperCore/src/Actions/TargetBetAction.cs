using Betkeeper.Enums;
using Betkeeper.Models;
using System.Linq;
using System.Collections.Generic;

namespace Betkeeper.Actions
{
    public class TargetBetAction
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

        public List<TargetBet> GetParticipatorsBets(int participatorId)
        {
            return TargetBetRepository.GetTargetBets(participatorId);
        }

        public List<TargetBet> GetCompetitionsTargetBets(int competitionId)
        {
            var targetIds = TargetRepository
                .GetTargets(competitionId: competitionId)
                .Select(target => target.TargetId)
                .ToList();

            return TargetBetRepository.GetTargetBets(targetIds: targetIds);
        }

        /// <summary>
        /// Adds target bets. Only allows adding bets which have a bet provided
        /// </summary>
        /// <param name="competitionId"></param>
        /// <param name="userId"></param>
        /// <param name="targetBets"></param>
        /// <exception cref="ActionException"></exception>
        public void SaveTargetBets(
            int competitionId, 
            int userId,
            List<TargetBet> targetBets)
        {
            var competition = CompetitionRepository.GetCompetition(competitionId);

            if (competition == null 
                || competition.State != Enums.CompetitionState.Open)
            {
                throw new ActionException(
                    ActionResultType.Conflict, 
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
                    ActionResultType.Unauthorized,
                    "User not in competition");
            }

            for (var i = 0; i < targetBets.Count; i++)
            {
                ValidateTarget(targetBets[i], competitionTargets, i);
            }

            SetParticipatorToTargetBets(targetBets, participator);

            AddOrUpdateTargetBets(participator.ParticipatorId, targetBets);
        }

        /// <summary>
        /// Add or update target bets.
        /// </summary>
        /// <param name="participator"></param>
        /// <param name="targetBets"></param>
        private void AddOrUpdateTargetBets(
            int participator,
            List<TargetBet> targetBets)
        {
            // Get existing target bets to know which ones to update
            var existingTargetBets = TargetBetRepository
                .GetTargetBets(participator: participator);

            var insertList = new List<TargetBet>();

            foreach (var targetBet in targetBets)
            {
                var existingTargetBet = existingTargetBets.SingleOrDefault(oldTargetBet =>
                    oldTargetBet.Target == targetBet.Target);

                if (existingTargetBet != null)
                {
                    existingTargetBet.Bet = targetBet.Bet;
                }
                else
                {
                    insertList.Add(targetBet);
                }
            }

            TargetBetRepository.AddTargetBets(insertList);
            TargetBetRepository.UpdateTargetBets(existingTargetBets);
        }

        /// <summary>
        /// Set participator to target bets.
        /// </summary>
        private static void SetParticipatorToTargetBets(
            List<TargetBet> targetBets, 
            Participator participator)
        {
            targetBets.ForEach(targetBet =>
            {
                targetBet.Participator = participator.ParticipatorId;
            });
        }

        /// <summary>
        /// Validate bet target
        /// </summary>
        /// <param name="targetBet"></param>
        /// <param name="competitionTargets"></param>
        /// <param name="targetIndex"></param>
        private static void ValidateTarget(
            TargetBet targetBet, 
            List<Target> competitionTargets,
            int targetIndex)
        {
            var target = competitionTargets.FirstOrDefault(
                competitionTarget => competitionTarget.TargetId == targetBet.Target);

            if (target == null)
            {
                throw new ActionException(
                    ActionResultType.Conflict,
                    "Target does not exist");
            }

            if (target.Type == TargetType.Result)
            {
                var scores = targetBet.Bet.Split('-');

                if (scores.Length != 2)
                {
                    throw new ActionException(
                        ActionResultType.InvalidInput,
                        $"Target {targetIndex + 1} has invalid result");
                }

                if (!int.TryParse(scores[0], out int homescore) 
                    || !int.TryParse(scores[1], out int awayscore))
                {
                    throw new ActionException(
                        ActionResultType.InvalidInput,
                        $"Target {targetIndex + 1} has invalid result");
                }
            }

            else if (target.Type == Enums.TargetType.Selection)
            {
                if (!target.Selections.Contains(targetBet.Bet))
                {
                    throw new ActionException(
                        ActionResultType.InvalidInput,
                        $"Target {targetIndex + 1} has invalid selection");
                }
            }
        }
    }
}
