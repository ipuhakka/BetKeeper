using Betkeeper.Page.Components;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Betkeeper.Pages.CompetitionPage
{
    public partial class CompetitionPage
    {
        public Tab GetResultsTab(int competitionId)
        {
            var scores = TargetAction.CalculateCompetitionPoints(competitionId);

            var pointOverviewTable = new StaticTable(header: new Row(new List<string> { "User", "Points" }, CellStyle.Bold))
            {
                // Sort overview table in descending order by total points
                Rows = scores.UserPointsDictionary.OrderByDescending(kvp => kvp.Value)
                .Select(kvp => new Row(new List<string> { kvp.Key, kvp.Value.ToString() }))
                .ToList()
            };

            // Create a more specific table
            var tableHeaderItems = new List<string>
            {
                "Bet",
                "Points",
                "Result"
            };

            tableHeaderItems.AddRange(scores.UserPointsDictionary.Keys.ToList());

            // Add header row (users) in constructor
            var table = new StaticTable(header: 
                new Row(
                    tableHeaderItems,
                    style: CellStyle.Bold));

            var summaryRowCellValues = new List<string> { "Total points", scores.MaximumPoints.ToString(), "-" };

            summaryRowCellValues.AddRange(scores.UserPointsDictionary.Values.Select(value => value.ToString()));

            // Add summary (users points)
            var summaryRow = new Row(
                summaryRowCellValues,
                CellStyle.Bold);
            table.Rows.Add(summaryRow);

            // Add data. Question first, then bets in same order as in header
            scores.TargetItems.ForEach(target =>
            {
                var row = new Row();

                row.Cells.Add(new Cell
                {
                    Style = CellStyle.Bold,
                    Value = target.Question
                });

                row.Cells.Add(new Cell
                {
                    Style = CellStyle.Bold,
                    Value = target.PointsAvailable
                });

                row.Cells.Add(new Cell
                {
                    Style = CellStyle.Bold,
                    Value = target.Result
                });

                target.BetItems.ForEach(bet =>
                {
                    var backgroundColor = bet.Result switch
                    {
                        Enums.TargetResult.CorrectResult => Color.Green,
                        Enums.TargetResult.CorrectWinner => Color.LightGreen,
                        Enums.TargetResult.Wrong => Color.Red,
                        _ => Color.Gray,
                    };

                    row.Cells.Add(new Cell
                    {
                        Color = backgroundColor,
                        Value = bet.Bet
                    });
                });

                table.Rows.Add(row);
            });

            return new Tab("resultsViewTab", "Results", new List<Component>
            {
                pointOverviewTable,
                table
            });
        }
    }
}
