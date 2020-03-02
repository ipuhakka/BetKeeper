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
}
