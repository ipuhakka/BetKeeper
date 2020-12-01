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
    public class TargetAction
    {
        private CompetitionRepository CompetitionRepository { get; set; }

        private ParticipatorRepository ParticipatorRepository { get; set; }

        private TargetRepository TargetRepository { get; set; }

        private TargetBetAction TargetBetAction { get; set; }

        private UserRepository UserRepository { get; set; }

        public TargetAction()
        {
            CompetitionRepository = new CompetitionRepository();
            ParticipatorRepository = new ParticipatorRepository();
            TargetRepository = new TargetRepository();
            TargetBetAction = new TargetBetAction();
            UserRepository = new UserRepository();
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
        /// Returns competition points
        /// </summary>
        /// <param name="competitionId"></param>
        /// <returns></returns>
        public CompetitionScores CalculateCompetitionPoints(int competitionId)
        {
            var targets = GetTargets(competitionId)
                .ToList();

            var targetBets = TargetBetAction.GetCompetitionsTargetBets(competitionId);

            var participators = ParticipatorRepository
                .GetParticipators(competitionId: competitionId);

            var users = UserRepository.GetUsersById(
                participators.Select(participator => participator.UserId).ToList());

            var competitionScores = new CompetitionScores
            {
                MaximumPoints = targets.Sum(target =>
                {
                    return target.Scoring.PointsForCorrectResult;
                }) ?? 0
            };

            participators
                .ForEach(participator =>
                {
                    var username = users.Single(user => user.UserId == participator.UserId).Username;
                    competitionScores.UserPointsDictionary.Add(username, 0);

                    foreach(var target in targets)
                    {
                        if (!competitionScores.TargetItems.Any(item => item.Question == target.Bet))
                        {
                            competitionScores.TargetItems.Add(
                                new CompetitionScores.TargetItem(target));
                        }

                        var targetBet = targetBets.SingleOrDefault(bet =>
                            bet.Target == target.TargetId
                            && bet.Participator == participator.ParticipatorId);

                        if (targetBet == null)
                        {
                            competitionScores
                                .TargetItems
                                .Single(targetItem => targetItem.Question == target.Bet)
                                .BetItems
                                .Add(new CompetitionScores.TargetItem.BetItem(
                                    TargetResult.DidNotBet, 
                                    null,
                                    username));

                            continue;
                        }

                        competitionScores
                            .TargetItems
                            .Single(targetItem => targetItem.Question == target.Bet)
                            .BetItems
                            .Add(new CompetitionScores.TargetItem.BetItem(
                                target.GetResult(targetBet), 
                                targetBet.Bet,
                                username));

                        competitionScores.UserPointsDictionary[username] += target.GetPoints(targetBet);
                    }
                });

            return competitionScores;
        }

        /// <summary>
        /// Validates targets
        /// </summary>
        /// <param name="targets"></param>
        /// <exception cref="ActionException"></exception>
        private void ValidateTargets(List<Target> targets)
        {
            var i = 0;

            if (targets.Select(target => target.Bet).Distinct().Count() != targets.Count)
            {
                throw new ActionException(
                    ActionExceptionType.InvalidInput,
                    "Two or more bets contain same question");
            }

            targets.ForEach(target =>
            {
                if (string.IsNullOrWhiteSpace(target.Bet))
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Row {i + 1}: No question given");
                }

                if (target.Scoring.PointsForCorrectResult == null)
                {
                    throw new ActionException(
                        ActionExceptionType.InvalidInput,
                        $"Row {i + 1}: Missing points for correct result");
                }

                if (target.Type == TargetType.Result 
                    && target.Scoring.PointsForCorrectWinner == null)
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

                i++;
            });
        }

        private bool ValidScoringForType(Target target)
        {
            switch (target.Type)
            {
                case TargetType.OpenQuestion:
                case TargetType.Selection:
                    return target.Scoring.PointsForCorrectWinner == null &&
                        target.Scoring.PointsForCorrectResult != null;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Class for presenting competition scores
        /// </summary>
        public class CompetitionScores
        {
            /// <summary>
            /// Usernames and users points
            /// </summary>
            public Dictionary<string, double> UserPointsDictionary { get; }

            /// <summary>
            /// Bet question and answers
            /// </summary>
            public List<TargetItem> TargetItems { get; }

            /// <summary>
            /// Maximum points possible to get in competition
            /// </summary>
            public double MaximumPoints { get; set; }

            public CompetitionScores()
            {
                UserPointsDictionary = new Dictionary<string, double>();
                TargetItems = new List<TargetItem>();
            }

            /// <summary>
            /// Target item
            /// </summary>
            public class TargetItem
            {
                /// <summary>
                /// Target question
                /// </summary>
                public string Question { get; set; }

                /// <summary>
                /// Actual result
                /// </summary>
                public string Result { get; set; }

                /// <summary>
                /// Points possible to get for bet
                /// </summary>
                private Scoring Scoring { get; set; }

                /// <summary>
                /// Get available points for a target item.
                /// </summary>
                public string PointsAvailable
                {
                    get
                    {
                        if (Scoring.PointsForCorrectWinner == null)
                        {
                            return $"{Scoring.PointsForCorrectResult}";
                        }

                        return $"Result: {Scoring.PointsForCorrectResult}, winner: {Scoring.PointsForCorrectWinner}";
                    }
                }

                /// <summary>
                /// Targets bets
                /// </summary>
                public List<BetItem> BetItems { get; set; }

                public TargetItem (Target target)
                {
                    var result = target?.Result?.Result ?? "-";

                    if (result == "UNRESOLVED-BET")
                    {
                        result = "-";
                    }

                    Result = result;
                    Question = target.Bet;
                    Scoring = target.Scoring;
                    BetItems = new List<BetItem>();
                }

                /// <summary>
                /// Bet item
                /// </summary>
                public class BetItem
                {
                    /// <summary>
                    /// Bet
                    /// </summary>
                    public string Bet { get; set; }

                    /// <summary>
                    /// Username
                    /// </summary>
                    public string User { get; set; }

                    /// <summary>
                    /// Result
                    /// </summary>
                    public TargetResult Result { get; set; }

                    public BetItem(TargetResult result, string bet, string user)
                    {
                        Result = result;
                        Bet = bet;
                        User = user;
                    }
                }
            }
        }
    }
}
