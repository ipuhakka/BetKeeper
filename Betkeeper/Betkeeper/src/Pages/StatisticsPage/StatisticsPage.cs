using Betkeeper.Actions;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Betkeeper.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Betkeeper.Pages.StatisticsPage
{
    public class StatisticsPage : PageBase
    {
        public override PageResponse GetPage(string pageKey, int userId)
        {
            return new PageResponse(
                pageKey, 
                new List<Component>
                {
                    new Chart<StatisticsChartData>(
                        "betStatisticsChart", 
                        StatisticsChartData.GetData(userId),
                        new ItemField("folder", DataType.String, "Folder"),
                        new List<ItemField>
                        {
                            new ItemField("verifiedReturn", DataType.Double, "Return %"),
                            new ItemField("profit", DataType.Double, "Profit"),
                            new ItemField("moneyPlayed", DataType.Double, "Money played"),
                            new ItemField("moneyWon", DataType.Double, "Money won"),
                            new ItemField("betsPlayed", DataType.Integer, "Bets played"),
                            new ItemField("betsWon", DataType.Integer, "Bets won"),
                            new ItemField("winPercentage", DataType.Double, "Win percentage"),
                            new ItemField("oddMean", DataType.Double, "Odd average"),
                            new ItemField("stakeMean", DataType.Double, "Bet average")
                        })
                },
                new Dictionary<string, object>());
        }

        private class StatisticsChartData
        {
            public string Folder { get; set; }

            public int BetsPlayed { get; set; }

            public int BetsWon { get; set; }

            public double WinPercentage { get; set; }

            public double OddMean { get; set; }

            public double StakeMean { get; set; }

            public double VerifiedReturn { get; set; }

            public double Profit { get; set; }

            public double MoneyPlayed { get; set; }

            public double MoneyWon { get; set; }

            /// <summary>
            /// Calculates data for statistics page
            /// </summary>
            /// <param name="userId"></param>
            /// <returns></returns>
            public static List<StatisticsChartData> GetData(int userId)
            {
                var data = new List<StatisticsChartData>
                {
                    GetStatisticsForFolder(userId, null)
                };

                data.AddRange(new FolderAction()
                        .GetUsersFolders(userId)
                        .Select(folder => GetStatisticsForFolder(userId, folder)));

                return data;
            }

            /// <summary>
            /// Returns statistics chart data for specific folder
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="folder"></param>
            private static StatisticsChartData GetStatisticsForFolder(int userId, string folder)
            {
                var bets = new BetAction().GetBets(userId, betFinished: true, folder: folder);

                if (bets.Count == 0)
                {
                    return new StatisticsChartData
                    {
                        Folder = folder ?? "Overview"
                    };
                }

                var wonBets = bets.Where(bet => bet.BetResult == Enums.BetResult.Won).ToList();
                var moneyWon = (double)wonBets.Sum(bet => bet.Stake * bet.Odd);
                var moneyPlayed = (double)bets.Sum(bet => bet.Stake);

                return new StatisticsChartData
                {
                    Folder = folder ?? "Overview",
                    BetsPlayed = bets.Count,
                    BetsWon = wonBets.Count,
                    OddMean = Math.Round(bets.Average(bet => bet.Odd ?? 0), 2),
                    StakeMean = Math.Round(bets.Average(bet => bet.Stake ?? 0), 2),
                    WinPercentage = Math.Round((double)wonBets.Count / bets.Count, 2),
                    VerifiedReturn = Math.Round(moneyWon / moneyPlayed, 2),
                    MoneyPlayed = Math.Round(moneyPlayed, 2),
                    MoneyWon = Math.Round(moneyWon, 2),
                    Profit = Math.Round(moneyWon - moneyPlayed, 2)
                };
            }
        }
    }
}
