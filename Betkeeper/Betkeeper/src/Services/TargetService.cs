using Betkeeper.Data;
using Betkeeper.Models;
using System.Collections.Generic;

namespace Betkeeper.Services
{
    /// <summary>
    /// Service class for operations on targets when repositories cannot be used directly
    /// </summary>
    public static class TargetService
    {
        public static List<Target> GetCompetitionTargets(int competitionId)
        {
            return new TargetRepository().GetTargets(competitionId);
        }
    }
}
