namespace Betkeeper.Enums
{
    public enum BetResult
    {
        Unresolved = -1,
        Lost = 0,
        Won = 1
    };

    public enum CompetitionState
    {
        Open = 0,
        Ongoing = 1,
        Finished = 2
    }

    public enum CompetitionRole
    {
        Participator = 0,
        Admin = 1,
        Host = 2
    }

    /// <summary>
    /// Scoring enum.
    /// </summary>
    public enum TargetScore
    {
        /// <summary>
        /// Match winner was correct.
        /// </summary>
        CorrectWinner = 0,

        /// <summary>
        /// Match result was correct.
        /// </summary>
        CorrectResult = 1
    }

    public enum TargetResult
    {
        Unresolved = 0,
        Wrong = 1,
        CorrectWinner = 2,
        CorrectResult = 3,
        DidNotBet = 4
    }

    public enum TargetType
    {
        /// <summary>
        /// Bet the result.
        /// </summary>
        Result,

        /// <summary>
        /// Select from options.
        /// </summary>
        Selection,

        /// <summary>
        /// Answer an open question.
        /// </summary>
        OpenQuestion
    }
}
