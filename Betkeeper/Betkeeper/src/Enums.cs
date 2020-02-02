namespace Betkeeper
{
    public class Enums
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
    }
}
